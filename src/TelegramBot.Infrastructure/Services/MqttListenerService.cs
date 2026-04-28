using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Telegram.Bot;
using TelegramBot.Domain.Settings;

namespace TelegramBot.Infrastructure.Services;

public class MqttListenerService(
    IConfiguration configuration,
    ITelegramBotClient botClient,
    IOptions<TelegramBotSettings> botSettings,
    ILogger<MqttListenerService> logger) : BackgroundService
{
    private readonly string _brokerHost = configuration["MQTT_BROKER_HOST"] ?? "message-broker";
    private readonly int _brokerPort = int.Parse(configuration["MQTT_BROKER_PORT"] ?? "1883");
    private readonly long _adminChatId = botSettings.Value.AdminChatId;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_adminChatId == 0)
        {
            logger.LogWarning("AdminChatId is not set. MQTT notifications will not be forwarded to Telegram.");
            return;
        }

        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_brokerHost, _brokerPort)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        mqttClient.DisconnectedAsync += OnDisconnected;

        async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            logger.LogInformation("Received MQTT message on topic {Topic}: {Payload}", e.ApplicationMessage.Topic, payload);

            try
            {
                await botClient.SendMessage(
                    chatId: _adminChatId,
                    text: payload,
                    cancellationToken: stoppingToken);

                logger.LogInformation("Forwarded MQTT message to Telegram Admin {AdminId}", _adminChatId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send Telegram message to {AdminId}", _adminChatId);
            }
        }

        async Task OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            if (stoppingToken.IsCancellationRequested) return;

            logger.LogWarning("Disconnected from MQTT broker. Retrying in 5 seconds...");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

                var resubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("#"))
                    .Build();
                await mqttClient.SubscribeAsync(resubscribeOptions, stoppingToken);
                logger.LogInformation("Re-subscribed to all topics (#) after reconnect");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to reconnect to MQTT broker.");
            }
        }

        try
        {
            logger.LogInformation("Connecting to MQTT broker at {Host}:{Port}...", _brokerHost, _brokerPort);
            await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic("#"))
                .Build();

            await mqttClient.SubscribeAsync(subscribeOptions, stoppingToken);
            logger.LogInformation("Subscribed to all topics (#)");

            // Keep the service alive
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Critical error in MqttListenerService.");
        }
    }
}
