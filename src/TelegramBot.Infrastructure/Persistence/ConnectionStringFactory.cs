using Microsoft.Extensions.Options;
using TelegramBot.Domain.Settings;

namespace TelegramBot.Infrastructure.Persistence;

public class ConnectionStringFactory(IOptions<TradingConnectionSettings> settings)
{
    public string Host => settings.Value.Host;
    public string Port => settings.Value.Port;
    public string Password => settings.Value.Password;
    
    public string ConnectionString => $"Host={Host};Port={Port};Database=trading;Username=trading;Password={Password}";
}