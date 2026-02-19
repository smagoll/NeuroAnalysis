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
    
    public async Task<string> Ask(string prompt)
    {
        var token = await _authService.GetAccessTokenAsync();

        var requestBody = new
        {
            model = "GigaChat",
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.7,
            max_tokens = 512
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
}