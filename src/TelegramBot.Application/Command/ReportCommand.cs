using Cortex.Mediator.Commands;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Domain.Messages.Common;

namespace TelegramBot.Application.Command;

public class ReportCommand : ICommand
{
    public required BrokerMessagePayload Payload  { get; set; }
}

public class ReportCommandHandler(IBffService bffService) : ICommandHandler<ReportCommand>
{
    public Task Handle(ReportCommand command, CancellationToken cancellationToken)
    {
        // ["quoteAsset"] = _quoteAsset,
        // ["periodKey"] = command.PeriodKey,
        // ["periodLabel"] = command.PeriodLabel,
            
        
        throw new NotImplementedException();
    }
}