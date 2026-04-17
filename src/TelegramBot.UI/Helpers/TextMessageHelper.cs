using Telegram.Bot.Types;
using TelegramBot.Application.Command;
using TelegramBot.Domain.Enums;

namespace TelegramBot.UI.Helpers;

public static class TextMessageHelper
{
    private static readonly Dictionary<string, BotCommandType> CommandMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/start"]     = BotCommandType.Start,
        ["/status"]    = BotCommandType.Status,
        ["/stats"]     = BotCommandType.Stats,
        ["/pnl"]       = BotCommandType.Pnl,
        ["/positions"] = BotCommandType.Positions,
        ["/trades"]    = BotCommandType.Trades,
        ["/agents"]    = BotCommandType.Agents,
        ["/cluster"]   = BotCommandType.Cluster,
        ["/help"]      = BotCommandType.Help,
        ["/alert"]     = BotCommandType.Alert,
    };

    private static readonly Dictionary<string, BotCommandType> CallbackMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["status"]              = BotCommandType.Status,
        ["stats"]               = BotCommandType.Stats,
        ["pnl"]                 = BotCommandType.Pnl,
        ["positions"]           = BotCommandType.Positions,
        ["trades"]              = BotCommandType.Trades,
        ["agents"]              = BotCommandType.Agents,
        ["cluster"]             = BotCommandType.Cluster,
        ["killswitch_confirm"]  = BotCommandType.KillSwitchConfirm,
        ["killswitch_execute"]  = BotCommandType.KillSwitchExecute,
        ["killswitch_cancel"]   = BotCommandType.KillSwitchCancel,
    };

    /// <summary>Converts a text message into a <see cref="TelegramCommand"/>.</summary>
    public static TelegramCommand ToCommand(this Message message)
    {
        var text     = message.Text?.Trim() ?? string.Empty;
        var chatId   = message.Chat.Id;
        var userId   = message.From?.Id ?? 0;

        if (!text.StartsWith('/'))
            return new TelegramCommand(BotCommandType.GeneralText, chatId, userId);

        var parts   = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var keyword = parts[0].Split('@')[0]; // strip @BotName suffix if present
        var args    = parts[1..].Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();

        return CommandMap.TryGetValue(keyword, out var commandType)
            ? new TelegramCommand(commandType, chatId, userId, args)
            : new TelegramCommand(BotCommandType.GeneralText, chatId, userId);
    }

    /// <summary>Converts a callback query into a <see cref="TelegramCommand"/>.</summary>
    public static TelegramCommand ToCommand(this CallbackQuery query)
    {
        var data   = query.Data ?? string.Empty;
        var chatId = query.Message?.Chat.Id ?? 0;
        var userId = query.From.Id;

        return CallbackMap.TryGetValue(data, out var commandType)
            ? new TelegramCommand(commandType, chatId, userId)
            : new TelegramCommand(BotCommandType.GeneralText, chatId, userId);
    }
}
