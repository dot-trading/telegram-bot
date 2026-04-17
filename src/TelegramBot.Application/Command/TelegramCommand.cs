using Cortex.Mediator.Commands;
using Microsoft.Extensions.Options;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Domain.Enums;
using TelegramBot.Domain.Messages;
using TelegramBot.Domain.Settings;

namespace TelegramBot.Application.Command;

public class TelegramCommand(BotCommandType commandName, long chatId, long fromUserId) : ICommand<BotResponse>
{
    public TelegramCommand(BotCommandType commandName, long chatId, long fromUserId, params string[] commandArgs)
        : this(commandName, chatId, fromUserId)
    {
        CommandArgs = commandArgs;
    }

    public BotCommandType CommandName { get; } = commandName;
    public string[] CommandArgs { get; } = [];
    public long ChatId { get; } = chatId;
    public long FromUserId { get; } = fromUserId;
}

public class TelegramCommandHandler(IMessageFormatter formatter, IOptions<TelegramBotSettings> settings)
    : ICommandHandler<TelegramCommand, BotResponse>
{
    private readonly long _adminChatId = settings.Value.AdminChatId;

    public async Task<BotResponse> Handle(TelegramCommand command, CancellationToken cancellationToken)
    {
        if (command.FromUserId != _adminChatId)
            return new BotResponse("⛔ Accès refusé.");

        return command.CommandName switch
        {
            BotCommandType.Start            => GetStartResponse(),
            BotCommandType.Status           => new BotResponse(await formatter.GetStatusMessageAsync()),
            BotCommandType.Stats            => new BotResponse(formatter.GetStatsMessage()),
            BotCommandType.Pnl              => new BotResponse(await formatter.GetPnlMessageAsync()),
            BotCommandType.Positions        => new BotResponse(await formatter.GetPositionsMessageAsync()),
            BotCommandType.Trades           => new BotResponse(formatter.GetTradesMessage(GetTradesLimit(command.CommandArgs))),
            BotCommandType.Agents           => new BotResponse(await formatter.GetAgentsMessageAsync()),
            BotCommandType.Cluster          => new BotResponse(formatter.GetClusterMessage()),
            BotCommandType.Help             => new BotResponse(GetHelpText()),
            BotCommandType.Alert            => new BotResponse(GetAlertText(command.CommandArgs)),
            BotCommandType.KillSwitchConfirm => GetKillSwitchConfirmResponse(),
            BotCommandType.KillSwitchExecute => new BotResponse(
                $"🛑 <b>KILL SWITCH ACTIVÉ</b>\n\n" +
                $"Toutes les opérations ont été arrêtées.\n" +
                $"⏱ {DateTime.Now:dd/MM/yyyy HH:mm:ss}"),
            BotCommandType.KillSwitchCancel  => new BotResponse("✅ Annulé. Système toujours actif."),
            _                               => new BotResponse("❓ Commande inconnue. Utilise /help pour voir les commandes disponibles."),
        };
    }

    private static BotResponse GetStartResponse() => new(
        $"🤖 <b>TRADING AI CONTROL CENTER</b>\n" +
        $"━━━━━━━━━━━━━━━━━━━━\n" +
        $"Bienvenue, <b>Hamza</b> 👋\n" +
        $"📅 {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n\n" +
        $"Utilise les boutons ou les commandes :",
        InlineKeyboard:
        [
            [("📊 Status", "status"),    ("📈 Stats", "stats")],
            [("📂 Positions", "positions"), ("📜 Trades", "trades")],
            [("🤖 Agents", "agents"),    ("☸️ Cluster", "cluster")],
            [("💰 P&L", "pnl"),          ("🚨 Kill Switch", "killswitch_confirm")],
        ]);

    private static BotResponse GetKillSwitchConfirmResponse() => new(
        "⚠️ <b>KILL SWITCH</b>\n\n" +
        "Tu es sur le point d'arrêter <b>TOUTES</b> les opérations.\n" +
        "Cette action est <b>irréversible</b> jusqu'à redémarrage manuel.\n\n" +
        "Confirmer ?",
        InlineKeyboard:
        [
            [("✅ CONFIRMER ARRÊT", "killswitch_execute"), ("❌ Annuler", "killswitch_cancel")],
        ]);

    private static int GetTradesLimit(string[] args) =>
        args.Length > 0 && int.TryParse(args[0], out var n) && n > 0 ? n : 5;

    private static string GetHelpText() =>
        "📖 <b>COMMANDES DISPONIBLES</b>\n\n" +
        "📊 <b>Monitoring</b>\n" +
        "/status — État global du système\n" +
        "/stats — Statistiques détaillées\n" +
        "/pnl — Résumé rapide du PnL actuel\n" +
        "/positions — Positions ouvertes\n" +
        "/trades [N] — Derniers N trades\n" +
        "/agents — État des agents IA\n" +
        "/cluster — État du cluster K8s\n\n" +
        "⚙️ <b>Contrôle</b>\n" +
        "/start — Menu principal\n" +
        "/alert &lt;msg&gt; — Test d'alerte\n" +
        "/help — Cette aide";

    private static string GetAlertText(string[] args) =>
        args.Length == 0
            ? "Usage: /alert &lt;message&gt;"
            : $"🔔 Alerte : {string.Join(" ", args)}";
}
