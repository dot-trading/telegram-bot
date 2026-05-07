using Cortex.Mediator.Commands;
using TelegramBot.Application.Common.Abstractions;
using TelegramBot.Domain.Messages.Common;

namespace TelegramBot.Application.Command;

/// <summary>
/// Traite les alertes opérationnelles directes publiées sur trading/alerts
/// (BUY, SELL, syncs Binance, etc.) — le message HTML est directement dans le payload.
/// </summary>
public class RawAlertCommand : ICommand
{
    public required BrokerMessagePayload Payload { get; set; }
    public long ChatId { get; set; }
}

public class RawAlertCommandHandler(
    ITelegramSender telegramSender) : ICommandHandler<RawAlertCommand>
{
    public async Task Handle(RawAlertCommand command, CancellationToken cancellationToken)
    {
        var html = command.Payload.MessageArgs.TryGetValue("html", out var h) ? h : string.Empty;
        if (string.IsNullOrWhiteSpace(html)) return;

        await telegramSender.SendAlertAsync(command.ChatId, html, cancellationToken);
    }
}
