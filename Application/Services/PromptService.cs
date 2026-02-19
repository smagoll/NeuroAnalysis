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
        var prompt = BuildObjectDetectionPrompt();

        var response = await _llmService.AskWithImageAsync(image, prompt);

        return ParseList(response);
    }

    public async Task<Dictionary<string, List<string>>> AnalyzeMaterialsAsync(
        byte[] image,
        List<string> detectedObjects)
    {
        if (detectedObjects == null || detectedObjects.Count == 0)
            return new Dictionary<string, List<string>>();

        var prompt = BuildMaterialPrompt(detectedObjects);

        var response = await _llmService.AskWithImageAsync(image, prompt);

        return ParseDictionary(response);
    }
    
    private string BuildObjectDetectionPrompt()
    {
        return """
               Ты система компьютерного зрения.

               Задача:
               Найди ВСЕ реальные физические объекты на изображении.

               Важно:
               - Только реальные предметы
               - Не сцена
               - Не фон
               - Не освещение
               - Не "room", "interior", "photo"

               Правила ответа:
               - Только существительные
               - На английском
               - Без пояснений
               - Без текста
               - Без markdown
               - Только JSON

               Верни СТРОГО JSON массив строк:

               ["object1","object2","object3"]

               Если объектов нет — верни [].
               """;
    }
    
    private string BuildMaterialPrompt(List<string> objects)
    {
        var list = string.Join(", ", objects);

        return $$$"""
                Ты эксперт по материалам.

                Дан список объектов:
                {{{list}}}

                Для КАЖДОГО объекта определи материалы изготовления.

                Правила:
                - Только материалы
                - Без описаний
                - Без комментариев
                - На английском
                - Только JSON
                - Не добавляй новые объекты

                Формат ответа:

                {
                "object": ["material1","material2"]
                }

                Пример:

                {
                "chair": ["wood","fabric"],
                "cup": ["ceramic"]
                }

                """;
    }
    
    private List<string> ParseList(string response)
    {
        var json = ExtractJson(response);

        try
        {
            var list = JsonConvert.DeserializeObject<List<string>>(json);

            return list ?? new List<string>();
        }
        catch
        {
            throw new Exception("LLM returned invalid object list JSON:\n" + response);
        }
    }

    private Dictionary<string, List<string>> ParseDictionary(string response)
    {
        var json = ExtractJson(response);

        try
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

            return dict ?? new Dictionary<string, List<string>>();
        }
        catch
        {
            throw new Exception("LLM returned invalid materials JSON:\n" + response);
        }
    }
    
    private string ExtractJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Empty LLM response");

        // удаляем ```json ``` если есть
        text = text.Replace("```json", "")
            .Replace("```", "")
            .Trim();

        int startObj = text.IndexOf('{');
        int startArr = text.IndexOf('[');

        int start;

        if (startObj == -1 && startArr == -1)
            throw new Exception("JSON not found in response:\n" + text);

        if (startObj == -1)
            start = startArr;
        else if (startArr == -1)
            start = startObj;
        else
            start = Math.Min(startObj, startArr);

        int endObj = text.LastIndexOf('}');
        int endArr = text.LastIndexOf(']');

        int end = Math.Max(endObj, endArr);

        if (end <= start)
            throw new Exception("Invalid JSON boundaries:\n" + text);

        return text.Substring(start, end - start + 1);
    }
}