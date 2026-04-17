using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Domain.Messages;
using TelegramBot.UI.Helpers;

namespace TelegramBot.UI.Controllers;

[ApiController]
public class UpdateController(
    ITelegramBotClient bot,
    IMediator mediator,
    ILogger<UpdateController> logger) : ControllerBase
{
    /// <summary>
    /// Telegram webhook endpoint. The URL segment must match the bot token
    /// (registered via SetWebhookAsync on startup) to prevent unauthorised calls.
    /// </summary>
    [HttpPost("api/{token}")]
    public async Task<IActionResult> PostAsync(
        [FromRoute] string token,
        [FromBody] Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            await HandleUpdateAsync(update, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error while processing update {UpdateId}", update.Id);
        }

        // Always return 200 so Telegram does not retry the update.
        return Ok();
    }

    private async Task HandleUpdateAsync(Update update, CancellationToken ct)
    {
        switch (update.Type)
        {
            case UpdateType.Message when update.Message?.Type == MessageType.Text:
                await HandleTextMessageAsync(update.Message, ct);
                break;

            case UpdateType.CallbackQuery when update.CallbackQuery is not null:
                await HandleCallbackQueryAsync(update.CallbackQuery, ct);
                break;

            default:
                logger.LogDebug("Ignored update type: {Type}", update.Type);
                break;
        }
    }

    private async Task HandleTextMessageAsync(Message message, CancellationToken ct)
    {
        var command  = message.ToCommand();
        var response = await mediator.SendCommandAsync(command, ct);
        var chatId   = message.Chat.Id;

        await bot.SendMessage(
            chatId,
            response.Text,
            parseMode: response.UseHtmlParsing ? ParseMode.Html : ParseMode.None,
            replyMarkup: BuildKeyboard(response),
            cancellationToken: ct);
    }

    private async Task HandleCallbackQueryAsync(CallbackQuery query, CancellationToken ct)
    {
        // Acknowledge the button press immediately.
        await bot.AnswerCallbackQuery(query.Id, cancellationToken: ct);

        var command  = query.ToCommand();
        var response = await mediator.SendCommandAsync(command, ct);
        var chatId   = query.Message!.Chat.Id;
        var msgId    = query.Message.MessageId;

        await bot.EditMessageText(
            chatId,
            msgId,
            response.Text,
            parseMode: response.UseHtmlParsing ? ParseMode.Html : ParseMode.None,
            replyMarkup: BuildKeyboard(response),
            cancellationToken: ct);
    }

    private static InlineKeyboardMarkup? BuildKeyboard(BotResponse response)
    {
        if (response.InlineKeyboard is null)
            return null;

        var rows = response.InlineKeyboard
            .Select(row =>
                row.Select(btn => InlineKeyboardButton.WithCallbackData(btn.Label, btn.Data)))
            .ToArray();

        return new InlineKeyboardMarkup(rows);
    }
}
