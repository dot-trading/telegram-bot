using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Application;
using TelegramBot.Application.Common.Abstractions;
using TelegramBot.Domain;
using TelegramBot.Domain.Settings;
using TelegramBot.Infrastructure;
using TelegramBot.UI.Services;

namespace TelegramBot.UI;

public static class DependencyInjection
{
    public static IServiceCollection AddTelegramBotServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Settings (auto-discovers all *Settings classes via reflection)
        services.ConfigureAllSettings(configuration);

        // Infrastructure (DbContext, DatabaseService, BinanceService, ClusterService, BFF client)
        services.AddInfrastructure(configuration);

        // Application (MessageFormatter, Mediator handlers)
        services.AddApplication();

        // Telegram bot client (singleton — one per process)
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<TelegramBotSettings>>().Value;
            return new TelegramBotClient(settings.BotToken);
        });

        // Telegram sender (envoie des alertes proactives depuis les handlers Application)
        services.AddSingleton<ITelegramSender, TelegramSenderService>();

        services.AddHostedService<MqttListenerService>();

        return services;
    }
}
