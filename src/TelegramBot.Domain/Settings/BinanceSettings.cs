using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBot.Domain.Settings;

public class BinanceSettings
{
    public string ApiKey { get; set; } = Environment.GetEnvironmentVariable("BINANCE_API_KEY") ?? string.Empty;
    public string ApiSecret { get; set; } = Environment.GetEnvironmentVariable("BINANCE_API_SECRET") ?? string.Empty;
    public string BaseUrl { get; set; } = "https://api.binance.com";

    public static IServiceCollection BindSettingsToProperties(IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<BinanceSettings>(configuration.GetSection("BinanceSettings"));
    }
}
