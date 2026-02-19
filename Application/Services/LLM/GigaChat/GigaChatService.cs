using System.Net.Http.Headers;
using System.Text;
using Application.Interfaces;
using Application.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application.Services.LLM.GigaChat;

public class GigaChatService : ILLMService
{
    private readonly HttpClient _http;
    private readonly GigaChatAuthService _authService;
    private readonly GigaChatOptions _options;

    public GigaChatService(HttpClient http, GigaChatAuthService authService, IOptions<GigaChatOptions> options)
    {
        _http = http;
        _authService = authService;
        _options = options.Value;
    }

    public async Task<string> AskWithImageAsync(byte[] image, string prompt)
    {
        var token = await _authService.GetAccessTokenAsync();
        var fileId = await UploadImageAsync(token, image);

        var request = CreateJsonRequest(_options.BaseUrl, token, new
        {
            model = _options.Model,
            messages = new[] { new { role = "user", content = prompt, attachments = new[] { fileId } } },
            stream = false,
            update_interval = 0
        });
        request.Headers.Add("RqUID", Guid.NewGuid().ToString());

        var parsed = await SendAsync<ChatResponse>(request, "Chat failed");
        return parsed.choices[0].message.content;
    }

    private async Task<string> UploadImageAsync(string token, byte[] image)
    {
        var imageContent = new ByteArrayContent(image);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var content = new MultipartFormDataContent
        {
            { imageContent, "file", "image.jpeg" },
            { new StringContent("general"), "purpose" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _options.FilesUrl)
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
            Content = content
        };

        var parsed = await SendAsync<FileUploadResponse>(request, "File upload failed");
        return parsed.id;
    }

    private HttpRequestMessage CreateJsonRequest(string url, string token, object body) =>
        new(HttpMethod.Post, url)
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
            Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
        };

    private async Task<T> SendAsync<T>(HttpRequestMessage request, string errorPrefix)
    {
        using var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"{errorPrefix}: {json}", null, response.StatusCode);

        return JsonConvert.DeserializeObject<T>(json)
               ?? throw new InvalidOperationException($"Invalid response: {json}");
    }
}

public class ChatResponse
{
    public Choice[] choices { get; set; } = [];
}

public class Choice
{
    public Message message { get; set; } = new();
}

public class Message
{
    public string content { get; set; } = "";
}

public class FileUploadResponse
{
    public string id { get; set; } = "";
}