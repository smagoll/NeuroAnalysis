using Application.Interfaces;

namespace Application.Services;

public class DownloadService : IDownloadService
{
    private readonly HttpClient _httpClient;

    public DownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<byte[]> DownloadImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("URL пустой");

        using var response = await _httpClient.GetAsync(imageUrl);

        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();

        return bytes;
    }
}