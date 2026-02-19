namespace Application.Interfaces;

public interface ILLMService
{
    Task<string>  AskWithImageAsync(byte[] image, string prompt);
}