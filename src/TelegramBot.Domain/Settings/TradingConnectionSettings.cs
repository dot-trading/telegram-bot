using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBot.Domain.Settings;

public class TradingConnectionSettings
{
    public string Host => Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    public string Port => Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    public string Password => Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "trading_secure_pwd_2026";

    public static IServiceCollection BindSettingsToProperties(IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<TradingConnectionSettings>(configuration.GetSection("TradingConnectionSettings"));
    }
}