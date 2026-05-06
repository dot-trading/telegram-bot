using System.Text;
using Cortex.Mediator;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Telegram.Bot;
using TelegramBot.Application.Command;
using TelegramBot.Domain.Settings;

namespace TelegramBot.UI.Services;

public class MqttListenerService : BackgroundService
{
    private readonly string _brokerHost;
    private readonly int _brokerPort;
    private readonly long _adminChatId;
    private readonly IMqttClient _client;
    private readonly ILogger<MqttListenerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MqttListenerService(
        IConfiguration configuration,
        ITelegramBotClient botClient,
        IOptions<TelegramBotSettings> botSettings,
        IServiceProvider serviceProvider,
        ILogger<MqttListenerService> logger)
    {
        var mqttFactory = new MqttFactory();
        _client = mqttFactory.CreateMqttClient();
        _adminChatId = botSettings.Value.AdminChatId;
        _brokerPort = int.Parse(configuration["MQTT_BROKER_PORT"] ?? "1883");
        _brokerHost = configuration["MQTT_BROKER_HOST"] ?? "message-broker";
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_adminChatId == 0)
        {
            _logger.LogWarning("AdminChatId is not set. MQTT notifications will not be forwarded to Telegram.");
            return;
        }
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_brokerHost, _brokerPort)
            .WithKeepAlivePeriod(TimeSpan.FromHours(1))
            .Build();

        _client.ApplicationMessageReceivedAsync += OnMessageReceived;
        _client.DisconnectedAsync += OnDisconnected;

        async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            _logger.LogInformation("Received MQTT message on topic {Topic}: {Payload}", e.ApplicationMessage.Topic, payload);

            try
            {
                await SendMessageAsync(
                    e.ApplicationMessage.Topic,
                    payload,
                    _adminChatId);
                    
                _logger.LogInformation("Forwarded MQTT message to Telegram Admin {AdminId}", _adminChatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Telegram message to {AdminId}", _adminChatId);
            }
            
            // if (!e.ApplicationMessage.Topic.Equals("trading/notifications"))
            // {
            //     // TODO: my be it is not necessary
            //     // await SendMessageAsync(
            //     //     e.ApplicationMessage.Topic,
            //     //     payload,
            //     //     e.ApplicationMessage.TopicAlias);
            // }
            // else
            // {
            //     try
            //     {
            //         await botClient.SendMessage(
            //             chatId: _adminChatId,
            //             text: payload,
            //             cancellationToken: stoppingToken);
            //
            //         logger.LogInformation("Forwarded MQTT message to Telegram Admin {AdminId}", _adminChatId);
            //     }
            //     catch (Exception ex)
            //     {
            //         logger.LogError(ex, "Failed to send Telegram message to {AdminId}", _adminChatId);
            //     }
            // }
        }

        async Task OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            if (stoppingToken.IsCancellationRequested) return;
            
            _logger.LogWarning("Disconnected from MQTT broker. Retrying in 5 seconds...");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            
            try
            {
                await _client.ConnectAsync(mqttClientOptions, stoppingToken);
            
                var resubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("#"))
                    .Build();
                await _client.SubscribeAsync(resubscribeOptions, stoppingToken);
                _logger.LogInformation("Re-subscribed to all topics (#) after reconnect");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reconnect to MQTT broker.");
            }
        }

        try
        {
            _logger.LogInformation("Connecting to MQTT broker at {Host}:{Port}...", _brokerHost, _brokerPort);
            await _client.ConnectAsync(mqttClientOptions, stoppingToken);

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(_ => Task.FromResult(true))
                .Build();

            await _client.SubscribeAsync(subscribeOptions, stoppingToken);
            _logger.LogInformation("Subscribed to all topics (#)");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Critical error in MqttListenerService.");
        }
    }

    private Task SendMessageAsync(string topic, string message, long chatId)
    {
        // await botClient.SendMessage(
        //     chatId: _adminChatId,
        //     text: payload,
        //     cancellationToken: stoppingToken);
        
        using var sessionScope = _serviceProvider.CreateScope();
        var mediator = sessionScope.ServiceProvider.GetRequiredService<IMediator>();

        return mediator.SendAsync(new BrokerMessageCommand(topic, message, chatId));
    }

    public override void Dispose()
    {
        _client.Dispose();
        base.Dispose();
    }
}
