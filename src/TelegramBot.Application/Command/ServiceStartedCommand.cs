using Cortex.Mediator.Commands;
using TelegramBot.Application.Common.Abstractions;
using TelegramBot.Domain.Messages.Common;

namespace TelegramBot.Application.Command;

/// <summary>
/// Reçoit une notification de démarrage de service de l'orchestrateur (via MQTT)
/// et envoie une alerte Telegram simple (pas d'enrichissement BFF nécessaire).
/// </summary>
public class ServiceStartedCommand : ICommand
{
    public required BrokerMessagePayload Payload { get; set; }
    public long ChatId { get; set; }
}

public class ServiceStartedCommandHandler(
    ITelegramSender telegramSender) : ICommandHandler<ServiceStartedCommand>
{
    public async Task Handle(ServiceStartedCommand command, CancellationToken cancellationToken)
    {
        var args        = command.Payload.MessageArgs;
        var quoteAsset  = args.TryGetValue("quoteAsset",  out var qa) ? qa : "?";
        var serviceName = args.TryGetValue("serviceName", out var sn) ? sn : "unknown-service";

        var message =
            $"🟢 <b>SERVICE DÉMARRÉ</b>\n" +
            $"━━━━━━━━━━━━━━━━━━━━\n" +
            $"⚙️ Service: <b>{serviceName}</b>\n" +
            $"💱 Asset: <b>{quoteAsset}</b>\n\n" +
            $"⚡ {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

        await telegramSender.SendAlertAsync(command.ChatId, message, cancellationToken);
    }
}
