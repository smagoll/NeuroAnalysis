using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IPhotoAnalysisService, PhotoAnalysisService>();
        services.AddScoped<IPromptService, MockPromptService>();

        return services;
    }
}