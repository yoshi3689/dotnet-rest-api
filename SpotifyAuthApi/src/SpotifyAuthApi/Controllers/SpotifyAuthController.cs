using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAuthApi.Services;

namespace SpotifyAuthApi.Controllers;

public class PlaylistCreate
{
    public string Name { get; }

    public bool? Public { get; set; }
    
    public string? Description { get; set; }

    // Assuming uris is an array of strings
    public string[] Uris { get; }
        
    public string UserId { get; }
}

[ApiController]
[Route("api/[controller]")]
public class SpotifyAuthController : ControllerBase
{
    // private readonly IHttpContextAccessor _context;
    private readonly ILogger<SpotifyAuthController> _logger;
    private readonly ISpotifyClientService _spotifyClientService;
    private const string TokenRetrieve = "UserAccessToken";
    private const string VerifierRetrieve = "SpotifyVerifier";

    public SpotifyAuthController(
        ISpotifyClientService spotifyClientService, 
        ILogger<SpotifyAuthController> logger
        ) {
        _spotifyClientService = spotifyClientService;
        _logger = logger;
    }

    // Generates a secure random verifier of length 100 and its challenge
    [HttpGet("login")]
    public IActionResult Login()
    {
        _logger.LogInformation(SpotifyClientService.ClientUrl);
        var (verifier, challenge) = PKCEUtil.GenerateCodes();
        
        // Store the verifier in a secure manner (e.g., session) for later use
        HttpContext.Session.SetString(VerifierRetrieve, verifier);
        string uri = _spotifyClientService.HandleLogin(challenge);
        
        Console.WriteLine("This is session id in login: " + HttpContext.Session.Id);
        
        // Redirect user to the Spotify login URI
        return Redirect(uri);
    }


    [HttpGet("callback")]
    public async Task<IActionResult> GetCallback([FromQuery] string code, [FromQuery] string? state)
    {
        // Retrieve the stored verifier based on the provided state (e.g., from session)
        var verifier = HttpContext.Session.GetString(VerifierRetrieve);
        
        // Exchange the authorization code for an access token
        if (verifier == null) return BadRequest("verifier is missing");
        var token = await _spotifyClientService.HandleCallback(code, verifier);
        
        // Store the access token in session
        HttpContext.Session.SetString(TokenRetrieve, token);
        
        // redirect the client to the homepage
        return RedirectPermanent($"{SpotifyClientService.ClientUrl}?login=true");
    }

    /// <summary>
    /// get user's profile
    /// </summary>
    /// <returns></returns>
    [HttpGet("user")]
    public async Task<IActionResult> GetUserProfile()
    { 
        _logger.LogInformation("user profile");
        SpotifyClient client = CreateClient();
        try
        {
            var profile = await client.UserProfile.Current();
            // var publicProfile = await client.Personalization.get;
            return Ok(profile);
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
        try
        {
            var noNull = AreAllPropertiesNonNull(pl);
            if (!noNull) throw new Exception("playlist argument contains null value");
            
            var client = CreateClient();
            
            // create playlist
            var plCreateRequest = new PlaylistCreateRequest(pl.Name);
            plCreateRequest.Description = pl.Description;
            plCreateRequest.Public = pl.Public;
            
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
            
            // TODO: save the added tracks to a database
            return Ok(new { mesage = "created playlist with items added" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("Error in creating/adding to a playlist ");
        }
    }
    
    /// <summary>
    /// in logging out, clear information in a session
    /// </summary>
    /// <returns></returns>
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