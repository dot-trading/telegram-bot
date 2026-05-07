using System.Text.Json;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using TelegramBot.Domain.Constants;
using TelegramBot.Domain.Messages.Common;

namespace TelegramBot.Application.Command;

/// <summary>
/// When receive a message, the parsing and message deserialization
/// will be executed in this handler 
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public record BrokerMessageCommand(string Topic, string Payload, long ChatId) : ICommand;

public class BrokerMessageCommandHandler(IMediator mediator) : ICommandHandler<BrokerMessageCommand>
{
    public async Task Handle(BrokerMessageCommand command, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Deserialize<BrokerMessagePayload>(command.Payload);
        if(payload == null) throw new ArgumentNullException(nameof(command.Payload));
        
        switch (command.Topic)
        {
            case MessageTopic.ReportTopicPath:
            {
                var reportCommand = new ReportCommand() { Payload = payload };
                await mediator.SendCommandAsync(reportCommand, cancellationToken);
            }
            break;

            case MessageTopic.MarketStressTopicPath:
            {
                var stressCommand = new MarketStressCommand
                {
                    Payload = payload,
                    ChatId  = command.ChatId,
                };
                await mediator.SendCommandAsync(stressCommand, cancellationToken);
            }
            break;

            case MessageTopic.ServiceStartTopicPath:
            {
                var startCommand = new ServiceStartedCommand
                {
                    Payload = payload,
                    ChatId  = command.ChatId,
                };
                await mediator.SendCommandAsync(startCommand, cancellationToken);
            }
            break;

            case MessageTopic.AlertsTopicPath:
            {
                var alertCommand = new RawAlertCommand
                {
                    Payload = payload,
                    ChatId  = command.ChatId,
                };
                await mediator.SendCommandAsync(alertCommand, cancellationToken);
            }
            break;
        }
    }
}


