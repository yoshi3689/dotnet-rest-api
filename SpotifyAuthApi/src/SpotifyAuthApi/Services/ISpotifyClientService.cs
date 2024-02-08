using SpotifyAPI.Web;

namespace SpotifyAuthApi.Services;

public interface ISpotifyClientService
{
    SpotifyClient GetSpotifyClient(string accessToken);
    Task<string> HandleCallback(string code, string verifier);
    string HandleLogin(string challenge);
}