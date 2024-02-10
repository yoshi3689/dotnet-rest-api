using SpotifyAPI.Web;

namespace SpotifyAuthApi.Services;

public class SpotifyClientService : ISpotifyClientService
{
    private static readonly String ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");
    private static readonly Uri CallbackUri = new Uri(Environment.GetEnvironmentVariable("CALLBACK_URI_DEV"));
    public static readonly String ClientUrl = Environment.GetEnvironmentVariable("CLIENT_URL");
    
    public SpotifyClient GetSpotifyClient(string accessToken)
    {
        return new SpotifyClient(accessToken);
    }

    public string HandleLogin(string challenge)
    {
        // Make sure  is in your applications redirect URIs!
        var loginRequest = new LoginRequest(
            CallbackUri,
            ClientId,
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
            new PKCETokenRequest(ClientId, code, CallbackUri, verifier)
        );
        
        return tokenResponse.AccessToken;
    }
}
