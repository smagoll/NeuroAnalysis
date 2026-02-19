using Application.Interfaces;
using Newtonsoft.Json;

namespace Application.Services;

public class PromptService : IPromptService
{
    private readonly ILLMService _llmService;

    public PromptService(ILLMService llmService)
    {
        _llmService =  llmService;
    }
    
    public async Task<List<string>> DetectObjectsAsync(byte[] image)
    {
        var prompt = Prompts.ObjectDetection;

        var response = await _llmService.AskWithImageAsync(image, prompt);

        return Parse<List<string>>(response);
    }

    public async Task<Dictionary<string, List<string>>> AnalyzeMaterialsAsync(
        byte[] image, 
        List<string> detectedObjects)
    {
        if (detectedObjects.Count == 0)
            return new Dictionary<string, List<string>>();
        
        var prompt = Prompts.MaterialAnalysis(detectedObjects);

        var response = await _llmService.AskWithImageAsync(image, prompt);

        return Parse<Dictionary<string, List<string>>>(response);
    }
    
    private T Parse<T>(string response)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(response)
                   ?? throw new InvalidOperationException("Empty deserialization result");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"LLM returned invalid JSON:\n{response}", ex);
        }
    }
}