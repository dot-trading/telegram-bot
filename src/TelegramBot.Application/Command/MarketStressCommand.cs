using Cortex.Mediator.Commands;
using TelegramBot.Application.Common.Abstractions;
using TelegramBot.Domain.Messages.Common;
using TradingProject.Bff.Client.Models.Dtos;
using TradingProject.Bff.Client.Services;

namespace TelegramBot.Application.Command;

/// <summary>
/// Receives a market-stress notification from the orchestrator (via MQTT),
/// queries the BFF for enriched context, then sends the Telegram alert.
/// </summary>
public class MarketStressCommand : ICommand
{
    public required BrokerMessagePayload Payload { get; set; }
    public long ChatId { get; set; }
}

public class MarketStressCommandHandler(
    IBffApiClient bffApiClient,
    ITelegramSender telegramSender) : ICommandHandler<MarketStressCommand>
{
    public async Task Handle(MarketStressCommand command, CancellationToken cancellationToken)
    {
        var args = command.Payload.MessageArgs;
        var quoteAsset = args.TryGetValue("quoteAsset", out var qa) ? qa : "?";
        var stressValue = args.TryGetValue("stressValue", out var sv) ? sv : "0";

        var stressInt = int.TryParse(stressValue, out var parsedStress) ? parsedStress : 0;

        // Fetch the enriched DTO from the BFF
        var context = await bffApiClient.GetMarketStressContextAsync(quoteAsset, stressInt, cancellationToken);

        // Fallback values if BFF is unavailable
        var fgIndex = context?.FearAndGreedIndex ?? 0;
        var fgLabel = context?.FearAndGreedLabel ?? "Unknown";
        var openCount = context?.OpenPositionsCount ?? 0;
        var dailyPnl = context?.DailyPnl ?? 0;
        var totalPnl = context?.TotalPnl ?? 0;

        var stressEmoji = stressInt switch
        {
            >= 80 => "🔴🔴🔴",
            >= 60 => "🔴🔴",
            >= 40 => "🟡",
            _ => "🟢",
        };

        var fgEmoji = fgIndex switch
        {
            <= 25 => "😱",
            <= 45 => "😰",
            <= 55 => "😐",
            <= 75 => "😄",
            _ => "🤑",
        };

        var pnlEmoji = dailyPnl >= 0 ? "📈" : "📉";

        var message =
            $"⚠️ <b>MARKET STRESS ALERT</b>\n" +
            $"━━━━━━━━━━━━━━━━━━━━\n" +
            $"💱 Asset: <b>{quoteAsset}</b>\n" +
            $"{stressEmoji} Stress level: <b>{stressValue}</b>\n\n" +
            $"📊 <b>Contexte marché :</b>\n" +
            $"├─ {fgEmoji} Fear &amp; Greed: {fgIndex} ({fgLabel})\n" +
            $"├─ 📂 Positions ouvertes: {openCount}\n" +
            $"├─ {pnlEmoji} P&amp;L jour ({quoteAsset}): {dailyPnl:+0.00;-0.00}\n" +
            $"└─ P&amp;L total ({quoteAsset}): {totalPnl:+0.00;-0.00}\n\n" +
            $"⚡ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

        await telegramSender.SendAlertAsync(command.ChatId, message, cancellationToken);
    }
}
