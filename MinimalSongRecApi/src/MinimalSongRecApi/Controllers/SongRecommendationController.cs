using Microsoft.AspNetCore.Mvc;
using MinimalSongRecApi.Services;
using MinimalSongRecApi.Utilities;
using SpotifyAPI.Web;

namespace MinimalSongRecApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SongRecommendationController : ControllerBase
{
    private readonly ILogger<SongRecommendationController> _logger;
    private readonly SpotifyService _spotify;
    public SongRecommendationController(ILogger<SongRecommendationController> logger)
    {
        _logger = logger;
        _spotify = SpotifyService.CreateServiceAsync().Result;
    }

    /// <summary>
    /// class for test post request, remove later
    /// </summary>
    public class TrackRequest
    {
        public string Name;

        public TrackRequest(string name)
        {
            Name = name;
        }
    }
    

    /// <summary>
    /// get request test
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    [HttpGet("trackInfo/{trackId}/")]
    public async Task<FullTrack> GetTrackById(string trackId)
    {
        var track = await _spotify.GetTrack(trackId);
        _logger.LogInformation(track.ToString());
        return track;
    }
    
        
    /// <summary>
    /// post request test
    /// </summary>
    /// <param name="trackData"></param>
    /// <returns></returns>
    [HttpPost("trackInfo/")]
    public string PostTrackId([FromBody] TrackRequest trackData)
    {
        _logger.LogInformation(trackData.ToString());
        return trackData.Name;
    }
    
    
    
    [HttpGet("tracks/search")]
    public async Task<IActionResult> FindTracks([FromQuery] string name)
    {
        try{
            var res = await _spotify.SearchTrack(name);
            _logger.LogInformation(res.ToString());
            return Ok(res); 
        }
        catch (HttpRequestException httpException) 
        {
            // Handle HTTP request error
            Console.WriteLine($"HTTP request error: {httpException.Message}");
            return BadRequest($"HTTP request error: {httpException.Message}"); 
        }
    }
    
    [HttpGet("artists/search")]
    public async Task<IActionResult> FindArtists([FromQuery] string name)
    {
        try{
            var res = await _spotify.SearchArtist(name);
            _logger.LogInformation(res.ToString());
            return Ok(res); 
        }
        catch (HttpRequestException httpException) 
        {
            // Handle HTTP request error
            Console.WriteLine($"HTTP request error: {httpException.Message}");
            return BadRequest($"HTTP request error: {httpException.Message}"); 
        }
    }

    //api/person/byName?firstName=a&lastName=b
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("recommendations")]
    public async Task<IActionResult> SongRecommendation([FromQuery] SongRecommendationParameters parameters)
    {
        try
        {
            _logger.LogInformation(parameters.ToString());
            return Ok(await _spotify.GetSongRecommendations(parameters));
        }
        catch (HttpRequestException httpException)
        {
            // Handle HTTP request error
            Console.WriteLine($"HTTP request error: {httpException.Message}");
            return BadRequest($"HTTP request error: {httpException.Message}");
        }
    }
}