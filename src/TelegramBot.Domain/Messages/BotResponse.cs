namespace TelegramBot.Domain.Messages;

/// <summary>
/// Represents a response to be sent back via Telegram, decoupled from the Telegram.Bot library.
/// </summary>
/// <param name="Text">The message text (supports HTML when UseHtmlParsing is true).</param>
/// <param name="UseHtmlParsing">Whether to parse the text as HTML (default: true).</param>
/// <param name="InlineKeyboard">Optional inline keyboard buttons as rows of (label, callbackData) pairs.</param>
public record BotResponse(
    string Text,
    bool UseHtmlParsing = true,
    IReadOnlyList<IReadOnlyList<(string Label, string Data)>>? InlineKeyboard = null);
