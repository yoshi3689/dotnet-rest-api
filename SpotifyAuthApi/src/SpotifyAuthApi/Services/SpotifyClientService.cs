using SpotifyAPI.Web;

namespace SpotifyAuthApi.Services;

/// <summary>
/// Spotify client 
/// </summary>
public class SpotifyClientService : ISpotifyClientService
{
    /// <summary>
    /// spotify client id
    /// </summary>
    private readonly String _clientId = Environment.GetEnvironmentVariable("CLIENT_ID");

    /// <summary>
    /// URL that a spotify callback goes to 
    /// </summary>
    private readonly String _callbackUri = Environment.GetEnvironmentVariable("CALLBACK_URI_PROD");
    
    /// <summary>
    /// create a spotify client with access token issued after user login
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public SpotifyClient GetSpotifyClient(string accessToken)
    {
        return new SpotifyClient(accessToken);
    }

    /// <summary>
    /// generates a callback uri and set access scopes
    /// </summary>
    /// <param name="challenge"></param>
    /// <returns>call back uri</returns>
    public string HandleLogin(string challenge)
    {
        Console.WriteLine(_callbackUri);
        // Make sure  is in your applications redirect URIs!
        var loginRequest = new LoginRequest(
            new Uri(_callbackUri),
            _clientId,
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
                Scopes.UserReadRecentlyPlayed
            }
        };
        var uri = loginRequest.ToUri();
        return uri.ToString();
    }

    /// <summary>
    /// issues an access token with user info provided by spotify 
    /// </summary>
    /// <param name="code">client verification code</param>
    /// <param name="verifier">client verifier</param>
    /// <returns>access token</returns>
    public async Task<string> HandleCallback(string code, string verifier)
    {
        var tokenResponse = await new OAuthClient().RequestToken(
            new PKCETokenRequest(_clientId, code, new Uri(_callbackUri), verifier)
        );
        
        return tokenResponse.AccessToken;
    }
}
