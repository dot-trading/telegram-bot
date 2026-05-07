using Cortex.Mediator.Commands;
using TelegramBot.Application.Common.Abstractions;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Domain.Messages.Common;

namespace TelegramBot.Application.Command;

/// <summary>
/// Reçoit une notification de stress de marché de l'orchestrateur (via MQTT),
/// interroge le BFF pour enrichir le contexte, puis envoie l'alerte Telegram.
/// </summary>
public class MarketStressCommand : ICommand
{
    public required BrokerMessagePayload Payload { get; set; }
    public long ChatId { get; set; }
}

public class MarketStressCommandHandler(
    IBffService bffService,
    ITelegramSender telegramSender) : ICommandHandler<MarketStressCommand>
{
    public async Task Handle(MarketStressCommand command, CancellationToken cancellationToken)
    {
        var args        = command.Payload.MessageArgs;
        var quoteAsset  = args.TryGetValue("quoteAsset",  out var qa) ? qa : "?";
        var stressValue = args.TryGetValue("stressValue", out var sv) ? sv : "?";

        // Interroger le BFF pour enrichir le message (Fear & Greed, positions, P&L)
        var context = await bffService.GetMarketStressContextAsync(quoteAsset, cancellationToken);

        var stressInt = int.TryParse(stressValue, out var stressIntParsed) ? stressIntParsed : 0;
        var stressEmoji = stressInt switch
        {
            >= 80 => "🔴🔴🔴",
            >= 60 => "🔴🔴",
            >= 40 => "🟡",
            _     => "🟢",
        };

        var fgEmoji = context.FearAndGreedIndex switch
        {
            <= 25 => "😱",
            <= 45 => "😰",
            <= 55 => "😐",
            <= 75 => "😄",
            _     => "🤑",
        };

        var pnlEmoji = context.DailyPnl >= 0 ? "📈" : "📉";

        var message =
            $"⚠️ <b>MARKET STRESS ALERT</b>\n" +
            $"━━━━━━━━━━━━━━━━━━━━\n" +
            $"💱 Asset: <b>{quoteAsset}</b>\n" +
            $"{stressEmoji} Stress level: <b>{stressValue}</b>\n\n" +
            $"📊 <b>Contexte marché :</b>\n" +
            $"├─ {fgEmoji} Fear &amp; Greed: {context.FearAndGreedIndex} ({context.FearAndGreedLabel})\n" +
            $"├─ 📂 Positions ouvertes: {context.OpenPositionsCount}\n" +
            $"├─ {pnlEmoji} P&amp;L jour ({quoteAsset}): {context.DailyPnl:+0.00;-0.00}\n" +
            $"└─ P&amp;L total ({quoteAsset}): {context.TotalPnl:+0.00;-0.00}\n\n" +
            $"⚡ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

        await telegramSender.SendAlertAsync(command.ChatId, message, cancellationToken);
    }
}
