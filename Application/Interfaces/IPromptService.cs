namespace Application.Interfaces;

public interface IPromptService
{
    Task<List<string>> DetectObjectsAsync(byte[] image);
    Task<Dictionary<string, List<string>>> AnalyzeMaterialsAsync(byte[] image, List<string> detectedObjects);
}