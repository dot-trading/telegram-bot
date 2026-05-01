using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBot.Domain.Settings;

public class ClusterSettings
{
    public string OllamaUrl { get; set; } = Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";
    public string PersistenceServiceUrl { get; set; } = Environment.GetEnvironmentVariable("PERSISTENCE_SERVICE_URL") ?? "http://localhost:5000";

    public static IServiceCollection BindSettingsToProperties(IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<ClusterSettings>(configuration.GetSection("ClusterSettings"));
    }
}
