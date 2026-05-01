using Cortex.Mediator.Commands;

namespace TelegramBot.Application.Command;

/// <summary>
/// When receive a message, the parsing and message deserialization
/// will be executed in this handler 
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public record BrokerMessageCommand(string Topic, string Payload, ushort TopicAlias) : ICommand;

public class BrokerMessageCommandHandler : ICommandHandler<BrokerMessageCommand>
{
    public Task Handle(BrokerMessageCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}


