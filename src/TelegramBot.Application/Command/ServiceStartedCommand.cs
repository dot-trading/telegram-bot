using Cortex.Mediator.Commands;
using TelegramBot.Application.Common.Abstractions;
using TelegramBot.Domain.Messages.Common;
using TradingProject.Bff.Client.Models.Dtos;
using TradingProject.Bff.Client.Services;

namespace TelegramBot.Application.Command;

/// <summary>
/// Receives a service-start notification from the orchestrator (via MQTT),
/// queries the BFF for enriched context, then sends the Telegram alert.
/// </summary>
public class ServiceStartedCommand : ICommand
{
    public required BrokerMessagePayload Payload { get; set; }
    public long ChatId { get; set; }
}

public class ServiceStartedCommandHandler(
    IBffApiClient bffApiClient,
    ITelegramSender telegramSender) : ICommandHandler<ServiceStartedCommand>
{
    public async Task Handle(ServiceStartedCommand command, CancellationToken cancellationToken)
    {
        var args = command.Payload.MessageArgs;
        var quoteAsset = args.TryGetValue("quoteAsset", out var qa) ? qa : "?";
        var serviceName = args.TryGetValue("serviceName", out var sn) ? sn : "unknown-service";

        // Fetch the enriched DTO from the BFF
        var context = await bffApiClient.GetServiceStartContextAsync(quoteAsset, serviceName, cancellationToken);

        if (context is null)
        {
            // Fallback: send a basic message if the BFF is unavailable
            var fallbackMessage =
                $"🟢 <b>SERVICE DÉMARRÉ</b>\n" +
                $"━━━━━━━━━━━━━━━━━━━━\n" +
                $"⚙️ Service: <b>{serviceName}</b>\n" +
                $"💱 Asset: <b>{quoteAsset}</b>\n\n" +
                $"⚡ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            await telegramSender.SendAlertAsync(command.ChatId, fallbackMessage, cancellationToken);
            return;
        }

        var fgEmoji = context.FearAndGreedIndex switch
        {
            <= 25 => "😱",
            <= 45 => "😰",
            <= 55 => "😐",
            <= 75 => "😄",
            _ => "🤑",
        };

        var pnlEmoji = context.DailyPnl >= 0 ? "📈" : "📉";

        var message =
            $"🟢 <b>SERVICE DÉMARRÉ</b>\n" +
            $"━━━━━━━━━━━━━━━━━━━━\n" +
            $"⚙️ Service: <b>{context.ServiceName}</b>\n" +
            $"💱 Asset: <b>{context.QuoteAsset}</b>\n\n" +
            $"📊 <b>Snapshot marché :</b>\n" +
            $"├─ {fgEmoji} Fear &amp; Greed: {context.FearAndGreedIndex} ({context.FearAndGreedLabel})\n" +
            $"├─ 📂 Positions ouvertes: {context.OpenPositionsCount}\n" +
            $"├─ {pnlEmoji} P&amp;L jour ({context.QuoteAsset}): {context.DailyPnl:+0.00;-0.00}\n" +
            $"└─ P&amp;L total ({context.QuoteAsset}): {context.TotalPnl:+0.00;-0.00}\n\n" +
            $"⚡ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

        await telegramSender.SendAlertAsync(command.ChatId, message, cancellationToken);
    }
}
