using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Common.Abstractions;

namespace TelegramBot.UI.Services;

/// <summary>
/// Implémentation de ITelegramSender qui délègue à ITelegramBotClient.
/// Réside dans la couche UI car ITelegramBotClient est une dépendance externe.
/// </summary>
public class TelegramSenderService(ITelegramBotClient botClient, ILogger<TelegramSenderService> logger)
    : ITelegramSender
{
    public async Task SendAlertAsync(long chatId, string htmlMessage, CancellationToken ct = default)
    {
        try
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: htmlMessage,
                parseMode: ParseMode.Html,
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send Telegram alert to chat {ChatId}", chatId);
        }
    }
}
