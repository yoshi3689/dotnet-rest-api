using MinimalSongRecApi.Utilities;

namespace MinimalSongRecApi.Services;

// Services/SpotifyService.cs
using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;

public class SpotifyService
{
    private readonly SpotifyClient _spotify;
    private static readonly string CLIENT_ID = "";
    private static readonly string CLIENT_SECRET = "";
    private static string _accessToken = "";
    
    

    private SpotifyService(SpotifyClient client)
    {
        _spotify = client;
    }

    public static async Task<SpotifyService> CreateServiceAsync()
    {
        SpotifyService spotify = new SpotifyService(await CreateClient());
        return spotify;
    }

    private static async Task<SpotifyClient> CreateClient()
    {
        var config = SpotifyClientConfig.CreateDefault();
        var request = new ClientCredentialsRequest(CLIENT_ID, CLIENT_SECRET);
        var response = await new OAuthClient(config).RequestToken(request);
        _accessToken = response.AccessToken;
        Console.WriteLine("Created config");
        return new SpotifyClient(config.WithToken(response.AccessToken));
    }


    // private static SpotifyClientConfig UpdateBasicClientConfig()
    // {
    //     Console.WriteLine("About to create config");
    //     return SpotifyClientConfig
    //         .CreateDefault()
    //         .WithAuthenticator(new ClientCredentialsAuthenticator(
    //         CLIENT_ID, CLIENT_SECRET));
    // }

    public async Task<FullTrack> GetTrack(string trackId)
    {
        FullTrack track = await _spotify.Tracks.Get(trackId);
        return track;
    }
    
    public async Task<List<FullTrack>> SearchTrack(string keyword)
    {
        var req = new SearchRequest(SearchRequest.Types.Track, keyword);
        req.Limit = 7;
        SearchResponse res = await _spotify.Search
            .Item(req);
        return res.Tracks.Items ?? new List<FullTrack>();
    }
    
    public async Task<List<FullArtist>> SearchArtist(string keyword)
    {
        var req = new SearchRequest(SearchRequest.Types.Artist, keyword);
        req.Limit = 3;
        SearchResponse res = await _spotify.Search
            .Item(req);
        return res.Artists.Items ?? new List<FullArtist>();
    }
    
    public async Task<SongRecommendationResponse> GetSongRecommendations(SongRecommendationParameters parameters)
    {
        var res = await SongRecommendationRequest.GetRecommendations(parameters, _accessToken);
        Console.WriteLine(res?.Tracks.ToString());
        Console.WriteLine(res?.Seeds.ToString());
        return res ?? new SongRecommendationResponse();
    }
    
}
