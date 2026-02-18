namespace Application.Interfaces;

public interface IPhotoAnalysisService
{
    Task<(Guid PhotoId, List<string> Objects)> AnalyzePhotoAsync(string imageUrl);
    Task<Dictionary<string, List<string>>> AnalyzeMaterialsAsync(Guid photoId, List<string> objects);
}