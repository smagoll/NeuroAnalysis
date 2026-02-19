using System.Net.Http.Headers;
using System.Text;
using Application.Interfaces;
using Newtonsoft.Json;

namespace Application.Services.LLM.GigaChat;

public class GigaChatService : ILLMService
{
    private readonly HttpClient _http;
    private readonly GigaChatAuthService _authService;

    private const string ChatUrl =
        "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

    
    public GigaChatService(HttpClient http, GigaChatAuthService authService)
    {
        _http = http;
        _authService = authService;
    }
    
    public async Task<string> AskWithImageAsync(byte[] image, string prompt)
{
    var token = await _authService.GetAccessTokenAsync();

    // 1. Загружаем файл
    var fileId = await UploadImageAsync(token, image);

    // 2. Передаём file_id через attachments, а не через content
    var requestBody = new
    {
        model = "GigaChat-Pro",
        messages = new object[]
        {
            new
            {
                role = "user",
                content = prompt,           // просто строка, не массив
                attachments = new[] { fileId }  // идентификатор файла сюда
            }
        },
        stream = false,
        update_interval = 0
    };

    var json = JsonConvert.SerializeObject(requestBody);
    var request = new HttpRequestMessage(HttpMethod.Post, ChatUrl);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    request.Headers.Add("RqUID", Guid.NewGuid().ToString());
    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await _http.SendAsync(request);
    var result = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"Chat failed: {result}");

    dynamic parsed = JsonConvert.DeserializeObject(result)!;
    return parsed.choices[0].message.content;
}

private async Task<string> UploadImageAsync(string token, byte[] image)
{
    var request = new HttpRequestMessage(HttpMethod.Post,
        "https://gigachat.devices.sberbank.ru/api/v1/files");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var content = new MultipartFormDataContent();
    var imageContent = new ByteArrayContent(image);
    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
    content.Add(imageContent, "file", "image.jpeg");
    content.Add(new StringContent("general"), "purpose");
    request.Content = content;

    var response = await _http.SendAsync(request);
    var result = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"File upload failed: {result}");

    dynamic parsed = JsonConvert.DeserializeObject(result)!;
    return (string)parsed.id;
}
}