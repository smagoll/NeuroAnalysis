using Application.Interfaces;

namespace Application.Services;

public class DownloadService : IDownloadService
{
    public Task<byte[]> DownloadImageAsync(string imageUrl)
    {
        return Task.FromResult(new byte[0]);
    }
}