namespace GenreFinderApi.Controllers;

public class IChatCompletionsResponse
{
    public List<Choice> choices { get; set; }
    string model { get; set; }
}

public class Choice
{
    public string finishReason { get; set; }
    public Message message { get; set; }
}

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}