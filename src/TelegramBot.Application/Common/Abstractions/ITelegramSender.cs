namespace TelegramBot.Application.Common.Abstractions;

/// <summary>
/// Abstraction pour envoyer des messages proactifs (alertes, notifications) au chat admin Telegram.
/// Découple les handlers Application de l'implémentation ITelegramBotClient (couche UI).
/// </summary>
public interface ITelegramSender
{
    /// <summary>Envoie une alerte HTML au chat admin.</summary>
    Task SendAlertAsync(long chatId, string htmlMessage, CancellationToken ct = default);
}
