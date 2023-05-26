

using System.Diagnostics.Metrics;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

string apiKey = "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";

string endpoint = @"https://api.openai.com/v1/chat/completions";

List<Message> messages = new List<Message>();

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");


while ( true)
{
    Console.Write("User: ");
    var content = Console.ReadLine();

    if (content is not { Length: > 0 }) 
        break;

    var message = new Message() { Role = "user", Content = content };
    messages.Add(message);

    var requestData = new Request()
    {
        ModelId = "gpt-3.5-turbo",
        Messages = messages
    };

    using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"{(int)response.StatusCode} {response.StatusCode}");
        break;
    }

    ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();

    var choices = responseData?.Choices ?? new List<Choice>();
    if(choices.Count == 0)
    {
        Console.WriteLine("No choices were returned by the API");
        continue;
    }
    var choice = choices[0];
    var responseMessage = choice.Message;

    messages.Add(responseMessage);
    var responseText = responseMessage.Content.Trim();
    Console.WriteLine($"ChatGPT: {responseText}");
    Console.WriteLine();
}

class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";
    [JsonPropertyName("content")]
    public string Content { get; set; } = "";
}
class Request
{
    [JsonPropertyName("model")]
    public string ModelId { get; set; } = "";
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = new();
}

class ResponseData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    [JsonPropertyName("object")]
    public string Object { get; set; } = "";
    [JsonPropertyName("created")]
    public ulong Created { get; set; }
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; } = new();
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = new();
}

class Choice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("message")]
    public Message Message { get; set; } = new();
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = "";
}

class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}