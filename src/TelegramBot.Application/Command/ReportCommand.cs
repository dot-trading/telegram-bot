using Cortex.Mediator.Commands;
using TelegramBot.Domain.Messages.Common;
using TradingProject.Bff.Client.Services;

namespace TelegramBot.Application.Command;

public class ReportCommand : ICommand
{
    public required BrokerMessagePayload Payload { get; set; }
}

/// <summary>
/// Handles periodic report notifications by fetching enriched data from the BFF.
/// The actual message formatting is delegated to the consumer (Telegram sender).
/// </summary>
public class ReportCommandHandler(
    IBffApiClient bffApiClient) : ICommandHandler<ReportCommand>
{
    public async Task Handle(ReportCommand command, CancellationToken cancellationToken)
    {
        var args = command.Payload.MessageArgs;
        var quoteAsset = args.TryGetValue("quoteAsset", out var qa) ? qa : "?";
        var periodKey = args.TryGetValue("periodKey", out var pk) ? pk : "?";
        var periodLabel = args.TryGetValue("periodLabel", out var pl) ? pl : "?";

        // For now, just fetch a service-start context as a basic enrichment example.
        // Future iterations will implement dedicated report DTOs from the BFF.
        var context = await bffApiClient.GetServiceStartContextAsync(
            quoteAsset, $"Report-{periodLabel}", cancellationToken);

        // Context is available for building the message; actual formatting TBD.
        _ = context;
    }
}
