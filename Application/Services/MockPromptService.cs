using Application.Interfaces;

namespace Application.Services;

public class MockPromptService : IPromptService
{
    public Task<List<string>> DetectObjectsAsync(byte[] image)
    {
        var detectedObjects = new List<string>
        {
            "laptop",
            "cup",
            "mouse"
        };
        
        return Task.FromResult(detectedObjects);
    }

    public Task<Dictionary<string, List<string>>> AnalyzeMaterialsAsync(byte[] image, List<string> detectedObjects)
    {
        var response = new Dictionary<string, List<string>>
        {
            { "laptop", new List<string>{ "aluminum", "plastic", "glass" } },
            { "cup", new List<string>{ "ceramic" } },
            { "mouse", new List<string>{ "plastic", "rubber" } }
        };
        
        return Task.FromResult(response);
    }
}