using SpotifyAPI.Web;

namespace SpotifyAuthApi.Services;

public class SpotifyClientService : ISpotifyClientService
{
    private readonly String ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");

    private readonly String CallbackUri = Environment.GetEnvironmentVariable("CALLBACK_URI_PROD");
    // public static readonly string ClientUrl = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT").Equals("Development") ? Environment.GetEnvironmentVariable("CLIENT_URL") : Environment.GetEnvironmentVariable("CLIENT_URL_PROD");
    
    public SpotifyClient GetSpotifyClient(string accessToken)
    {
        return new SpotifyClient(accessToken);
    }

    public string HandleLogin(string challenge)
    {
        // Make sure  is in your applications redirect URIs!
        var loginRequest = new LoginRequest(
            new Uri(CallbackUri),
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
            new PKCETokenRequest(ClientId, code, new Uri(CallbackUri), verifier)
        );
        
        return tokenResponse.AccessToken;
    }
}
