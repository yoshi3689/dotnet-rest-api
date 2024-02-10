namespace GenreFinderApi.Controllers;

public interface IChatCompletionsResponse
{
    List<Choice> Choices { get; set; }
    string Model { get; set; }
}

public class Choice
{
    public string FinishReason { get; set; }
    public Message Message { get; set; }
}

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
}