namespace Application.Options;

public class GigaChatOptions
{
    public const string Section = "GigaChat";
    
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = "GIGACHAT_API_PERS";
}