namespace Application.Interfaces;

public interface IDownloadService
{
    Task<byte[]> DownloadImageAsync(string imageUrl);
}