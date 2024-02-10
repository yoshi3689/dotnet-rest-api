using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using GenreFinderApi.Services;

namespace GenreFinderApi.Controllers
{
    /// <summary>
    /// Controller for fetching chat completions from OpenAI API based on provided artists.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GenreFinderController : ControllerBase
    {
        private readonly ILogger<GenreFinderController> _logger;
        private readonly HttpClient _httpClient;
        private readonly GenrePromptGenerator _promptGenerator;

        /// <summary>
        /// Constructor for GenreFinderController.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="httpClient">The HTTP client instance.</param>
        /// <param name="promptGenerator">The genre prompt generator instance.</param>
        public GenreFinderController(ILogger<GenreFinderController> logger, HttpClient httpClient, GenrePromptGenerator promptGenerator)
        {
            _logger = logger;
            _httpClient = httpClient;
            _promptGenerator = promptGenerator;
        }

        /// <summary>
        /// Fetches chat completions from OpenAI API based on provided artists.
        /// </summary>
        /// <param name="artists">An array of artist names.</param>
        /// <returns>An action result containing the chat completions.</returns>
        [HttpPost("genres/find")]
        public async Task<IActionResult> FetchChatCompletions([FromBody] string[] artists)
        {
            _logger.LogInformation(artists[0]);
            try
            {
                // Generate prompt using the provided artists
                string prompt = _promptGenerator.GenerateChatCompletionPrompt(artists);
                _logger.LogInformation(prompt);
                foreach (var artist in artists)
                {
                    _logger.LogInformation(artist);
                }
                
                // Prepare the request body
                var requestBody = new
                {
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    model = "gpt-3.5-turbo"
                };

                // Serialize the request body to JSON
                var jsonRequestBody = JsonSerializer.Serialize(requestBody);

                // Prepare the HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
                {
                    Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json")
                };

                // Add authorization header
                var key = Environment.GetEnvironmentVariable("OPEN_AI_API_KEY");
                request.Headers.Add("Authorization", $"Bearer {key}");
                _logger.LogInformation(Environment.GetEnvironmentVariable("OPEN_AI_API_KEY"));
                // Send the HTTP request
                var response = await _httpClient.SendAsync(request);

                // Check if request was successful
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                    return StatusCode((int)response.StatusCode);
                }

                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                var responseObject = JsonSerializer.Deserialize<IChatCompletionsResponse>(responseContent);

                // Extract content from response
                var content = responseObject?.choices[0].message.content;

                // Return content
                return Ok(content?.Split(", ") ?? Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching chat completions.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
