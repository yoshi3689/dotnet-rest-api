using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAuthApi.Services;

namespace SpotifyAuthApi.Controllers;

/// <summary>
/// request body necessary for spotify playlist update/create
/// </summary>
public class User
{
    public PrivateUser Profile;
    public List<PlayHistoryItem> RecentItems;
}

/// <summary>
/// request body necessary for spotify playlist update/create
/// </summary>
public class PlaylistCreate
{
    /// <summary>
    /// name of paylist
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// playlist's visibility
    /// </summary>
    public bool? Public { get; set; }
    
    /// <summary>
    /// playlist's description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// tracks' uris to be included in the playlis
    /// </summary>
    public string[]? Uris { get; set;}
        
    /// <summary>
    /// spotify user id
    /// </summary>
    public string? UserId { get; set;}
}

/// <summary>
/// spotify oauth and auth-required method handler
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SpotifyAuthController : ControllerBase
{
    // private readonly IHttpContextAccessor _context;
    private readonly ILogger<SpotifyAuthController> _logger;
    private readonly ISpotifyClientService _spotifyClientService;
    private const string TokenRetrieve = "UserAccessToken";
    private const string VerifierRetrieve = "SpotifyVerifier";
    private const string RedirectUrl = "RedirectUrl";

    /// <summary>
    /// controller constructor
    /// </summary>
    /// <param name="spotifyClientService">custom singleton registered in Program.cs</param>
    /// <param name="logger">default logger</param>
    public SpotifyAuthController(
        ISpotifyClientService spotifyClientService, 
        ILogger<SpotifyAuthController> logger
        ) {
        _spotifyClientService = spotifyClientService;
        _logger = logger;
    }

    /// <summary>
    /// handle login request from a frontend
    /// </summary>
    /// <returns>callback uri</returns>
    [HttpGet("login")]
    public IActionResult Login()
    {
        _logger.LogInformation("login");
        // set the request origin to redirect the user to the frontend app 
        var refererUrl = Request.Headers["Referer"].ToString();
        HttpContext.Session.SetString(RedirectUrl, refererUrl);
        
        // one of the spotify auth methods
        var (verifier, challenge) = PKCEUtil.GenerateCodes();
        
        // Store the verifier (e.g., session) for later use
        HttpContext.Session.SetString(VerifierRetrieve, verifier);
        string uri = _spotifyClientService.HandleLogin(challenge);
        
        // Redirect user to the Spotify login URI
        return Redirect(uri);
    }


    /// <summary>
    /// handle the callback request invocation
    /// </summary>
    /// <param name="code">code challenge</param>
    /// <returns></returns>
    [HttpGet("callback")]
    public async Task<IActionResult> GetCallback([FromQuery] string code)
    {
        // Retrieve necessary values from session
        var verifier = HttpContext.Session.GetString(VerifierRetrieve);
        if (verifier == null) return StatusCode(500, "verifier is not restored from storage");
        var url = HttpContext.Session.GetString(RedirectUrl);
        if (url == null) return StatusCode(500, "redirect url is not restored from storage");
        
        // Exchange the authorization code for an access token
        var token = await _spotifyClientService.HandleCallback(code, verifier);
        HttpContext.Session.SetString(TokenRetrieve, token);
        
        // redirect the client to the homepage
        return RedirectPermanent($"{url}?login=true");
    }

    /// <summary>
    /// handle request for user profile fetch
    /// </summary>
    /// <returns>user profile</returns>
    [HttpGet("user")]
    public async Task<IActionResult> GetUserProfile()
    { 
        _logger.LogInformation("profile");
        try
        {
            SpotifyClient client = CreateClient();
            var profile = await client.UserProfile.Current();
            var recentlyPlayedTracks = await client.Player.GetRecentlyPlayed();
            var u = new User();
            u.Profile = profile;
            u.RecentItems = recentlyPlayedTracks.Items;
            // recentlyPlayedTracks.Items
            // var publicProfile = await client.Personalization.get;
            return Ok(u);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            return BadRequest("Error in getting your profile.");
        }
    }
    
    /// <summary>
    /// Create playlist and add provided items to it
    /// </summary>
    /// <param name="pl">playlistCreate instance propagated from the F.E.</param>
    /// <returns>message determining if the sequence of said operations succeeded </returns>
    /// <exception cref="Exception">playlistCreate instance's properties contain null value, playlist creation failed</exception>
    [HttpPost("playlist/create")]
    public async Task<IActionResult> CreateNewPlaylist([FromBody] PlaylistCreate pl)
    {
        _logger.LogInformation(pl.Name);
        try
        {
            var noNull = AreAllPropertiesNonNull(pl);
            if (!noNull) throw new Exception("playlist argument contains null value");
            
            var client = CreateClient();
            
            // create playlist
            var plCreateRequest = new PlaylistCreateRequest(pl.Name);
            plCreateRequest.Description = pl.Description;
            plCreateRequest.Public = true;
            
            var plCreationRes = await client.Playlists.Create(
                pl.UserId, 
                plCreateRequest
                );
            if (plCreationRes.Id == null) throw new Exception("playlist creation failed");
            
            // add to playlist
            await client.Playlists.AddItems(
                plCreationRes.Id,
                new PlaylistAddItemsRequest(pl.Uris)
            );
            
            return Ok(new { mesage = "created playlist with items added" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, pl);
        }
    }
    
    /// <summary>
    /// clears information in a session
    /// </summary>
    /// <returns>logout status message</returns>
    [HttpGet("logout")]
    public IActionResult Logout()
    { 
        _logger.LogInformation("logout");
        try
        {
            HttpContext.Session.Remove(TokenRetrieve);
            HttpContext.Session.Remove(VerifierRetrieve);
            // var publicProfile = await client.Personalization.get;
            return Ok(new { message = "cookies cleared. logout successful" });
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            return BadRequest("Error in logging out.");
        }
    }

    /// <summary>
    /// Create SpotifyClient instance with an access token
    /// </summary>
    /// <returns>SpotifyClient instance</returns>
    /// <exception cref="Exception">when an access token does not exist in the session object</exception>
    private SpotifyClient CreateClient()
    {
        // _logger.LogInformation(HttpContext.Session.Id);
        var accessToken = HttpContext.Session.GetString(TokenRetrieve);
        if (accessToken == null) throw new Exception("access token invalid. check if it's not expired");
        return new SpotifyClient(accessToken);
    } 
    
    /// <summary>
    /// check if all properties of a PlaylistCreate object(sent from F.E.) are NON-null
    /// </summary>
    /// <param name="obj">instance of PlaylistCreate from F.E.</param>
    /// <returns>if no properties contain null value</returns>
    private bool AreAllPropertiesNonNull(PlaylistCreate obj)
    {
        Type type = typeof(PlaylistCreate);
        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            
            object? value = property.GetValue(obj);
            _logger.LogInformation(value?.ToString());
            
            if (value == null)
            {
                return false; // If any property is null, return false
            }
        }

        return true; // All properties are non-null
    }
}