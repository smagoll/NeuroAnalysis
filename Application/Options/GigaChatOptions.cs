namespace Application.Options;

public class GigaChatOptions
{
    public const string Section = "GigaChat";
    
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = "GIGACHAT_API_PERS";
    public string BaseUrl { get; set; } = string.Empty;
    public string AuthUrl { get; set; } = string.Empty;
    public string FilesUrl { get; set; } = string.Empty;
    public string Model { get; set; } = "GigaChat-Pro";
}