using SpotifyAPI.Web;

namespace SpotifyAuthApi.Services;

public class SpotifyClientService : ISpotifyClientService
{
    private static String CLIENT_ID = Environment.GetEnvironmentVariable("CLIENT_ID");
    private static Uri CALLBACK_URI = new Uri(Environment.GetEnvironmentVariable("CALLBACK_URI_DEV"));
    public static String CLIENT_URL = Environment.GetEnvironmentVariable("CLIENT_URL");
    
    public SpotifyClient GetSpotifyClient(string accessToken)
    {
        return new SpotifyClient(accessToken);
    }

    public string HandleLogin(string challenge)
    {
        // Make sure  is in your applications redirect URIs!
        var loginRequest = new LoginRequest(
            CALLBACK_URI,
            CLIENT_ID,
            LoginRequest.ResponseType.Code
        )
        {
            CodeChallengeMethod = "S256",
            CodeChallenge = challenge,
            Scope = new[]
            {
                Scopes.PlaylistReadPrivate, 
                Scopes.PlaylistReadCollaborative,
                Scopes.PlaylistModifyPublic,
                Scopes.PlaylistModifyPrivate,
                Scopes.UserTopRead,
            }
        };
        var uri = loginRequest.ToUri();
        return uri.ToString();
    }

    public async Task<string> HandleCallback(string code, string verifier)
    {
        var tokenResponse = await new OAuthClient().RequestToken(
            new PKCETokenRequest(CLIENT_ID, code, CALLBACK_URI, verifier)
        );
        
        return tokenResponse.AccessToken;
    }
}
