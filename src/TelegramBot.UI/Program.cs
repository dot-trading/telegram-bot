using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Domain.Settings;
using TelegramBot.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddTelegramBotServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapControllers();

// Register the Telegram webhook on startup (skipped when BotToken is not configured).
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await using var scope = app.Services.CreateAsyncScope();
    var bot      = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
    var settings = scope.ServiceProvider.GetRequiredService<IOptions<TelegramBotSettings>>().Value;
    var logger   = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (string.IsNullOrWhiteSpace(settings.BotToken))
    {
        logger.LogWarning("TELEGRAM_BOT_TOKEN is not set — webhook registration skipped.");
        return;
    }

    if (string.IsNullOrWhiteSpace(settings.WebhookBaseUrl))
    {
        logger.LogWarning("WEBHOOK_BASE_URL is not set — webhook registration skipped.");
        return;
    }

    var webhookUrl = $"{settings.WebhookBaseUrl.TrimEnd('/')}/api/{settings.BotToken}";
    await bot.SetWebhook(webhookUrl);
    logger.LogInformation("Telegram webhook registered at {Url}", webhookUrl);
});

app.Run();
