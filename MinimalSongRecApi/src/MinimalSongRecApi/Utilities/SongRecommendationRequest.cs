using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonException = System.Text.Json.JsonException;

namespace MinimalSongRecApi.Utilities;

// using static HttpClient client = new();
// client.DefaultRequestHeaders.Accept.Clear();
// client.DefaultRequestHeaders.Accept.Add(
//     new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
// client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");


public static class SongRecommendationRequest
{
    private static readonly string RequestUrl = "https://api.spotify.com/v1/recommendations?";
    private static readonly HttpClient HttpClient = new ();
    
    // Other parameters (Instrumentalness, Key, Liveness, Loudness, Mode, Popularity, Speechiness, Tempo, Time Signature, Valence)
    // Add similar properties for each parameter

    private static SongRecommendationResponse? ParseJson(string serializedJson)
    {
        var contractResolver = new DefaultContractResolver();
        contractResolver.NamingStrategy = new SnakeCaseNamingStrategy(true, false);
        var settings = new JsonSerializerSettings
        {
            ContractResolver  = contractResolver
        };
        return JsonConvert.DeserializeObject<SongRecommendationResponse>(serializedJson, settings);
    }

    public static async Task<SongRecommendationResponse?> GetRecommendations(SongRecommendationParameters parameters, string token)
    {
        try
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage res = await HttpClient.GetAsync(RequestUrl + parameters);
            res.EnsureSuccessStatusCode();  // Ensure that the HTTP request was successful
            string serializedRes = await res.Content.ReadAsStringAsync();
            Console.WriteLine(serializedRes);
            var deserialized = ParseJson(serializedRes);
            Console.WriteLine(deserialized?.ToString());
            if (deserialized == null || deserialized.Seeds == null || deserialized.Tracks == null)
            {
                throw new JsonException("cannot parse JSON object from recommendations object");
            }

            return deserialized;
        }
        catch (JsonException jsonException)
        {
            Console.WriteLine($"Error deserializing JSON: {jsonException.Message}");
            return null;
        }
        
    } 
}
