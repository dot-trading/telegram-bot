using TelegramBot.Domain.Messages;

namespace TelegramBot.Domain.Abstractions;

public interface IBffService
{
    Task<PnlSummary> GetPnlSummaryAsync(string? spot = null, CancellationToken ct = default);

    /// <summary>
    /// Récupère le contexte enrichi pour la notification de stress de marché.
    /// Le BFF agrège Fear &amp; Greed, positions ouvertes et P&amp;L pour le quoteAsset donné.
    /// </summary>
    Task<MarketStressContext> GetMarketStressContextAsync(string quoteAsset, CancellationToken ct = default);
}
