using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBot.Domain.Settings;

public class TelegramBotSettings
{
    public string BotToken { get; set; } = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? string.Empty;
    public long AdminChatId { get; set; } = long.TryParse(Environment.GetEnvironmentVariable("TELEGRAM_ADMIN_CHAT_ID"), out var id) ? id : 0;
    public string WebhookBaseUrl { get; set; } = Environment.GetEnvironmentVariable("WEBHOOK_BASE_URL") ?? string.Empty;

    public static IServiceCollection BindSettingsToProperties(IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<TelegramBotSettings>(configuration.GetSection("TelegramBotSettings"));
    }
}
