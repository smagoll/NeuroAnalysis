using Application.Interfaces;
using Application.Options;
using Application.Services;
using Application.Services.LLM.GigaChat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IPhotoAnalysisService, PhotoAnalysisService>();
        services.AddScoped<IPromptService, PromptService>();
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<ILLMService, GigaChatService>();
        services.AddSingleton<GigaChatAuthService>();
        
        return services;
    }

    public static void SetupOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GigaChatOptions>(
            configuration.GetSection(GigaChatOptions.Section));
    }
}