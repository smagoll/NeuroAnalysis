using Application;
using Application.Interfaces;
using Application.Services;
using Application.Services.LLM.GigaChat;
using Infrastructure.Database;

namespace NeuroAnalysis;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddInfrastructureLayer(builder.Configuration);
        builder.Services.AddApplicationLayer();
        builder.Services.SetupOptions(builder.Configuration);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var disableSslValidation = () => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12
        };

        builder.Services.AddHttpClient<ILLMService, GigaChatService>()
            .ConfigurePrimaryHttpMessageHandler(disableSslValidation);

        builder.Services.AddHttpClient<GigaChatAuthService>()
            .ConfigurePrimaryHttpMessageHandler(disableSslValidation);
        
        builder.Services.AddHttpClient<IDownloadService, DownloadService>();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}