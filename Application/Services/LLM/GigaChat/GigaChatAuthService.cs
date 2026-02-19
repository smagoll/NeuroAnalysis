using System.Net.Http.Headers;
using System.Text;
using Application.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application.Services.LLM.GigaChat;

public class GigaChatAuthService
{
    private readonly HttpClient _http;
    private readonly GigaChatOptions _options;
    
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    
    private const string AuthUrl = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
    
    private readonly SemaphoreSlim _lock = new(1, 1);

    public GigaChatAuthService(HttpClient http, IOptions<GigaChatOptions> options)
    {
        _http = http;
        _options = options.Value;
    }
    
    public async Task<string> GetAccessTokenAsync()
    {
        if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
            return _cachedToken;

        await _lock.WaitAsync();
        try
        {
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
                return _cachedToken;

            var base64 = _options.ClientSecret;

            var request = new HttpRequestMessage(HttpMethod.Post, AuthUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64);
            request.Headers.Add("RqUID", Guid.NewGuid().ToString());
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "scope", _options.Scope }
            });

            var response = await _http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Auth failed: {json}");

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json)!;
            
            _cachedToken = tokenResponse.access_token;
            
            _tokenExpiry = DateTime.UtcNow.AddMinutes(29);

            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private class TokenResponse
    {
        public string access_token { get; set; } = string.Empty;
    }
}