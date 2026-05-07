namespace TelegramBot.Domain.Messages;

/// <summary>
/// Contexte enrichi pour la notification de stress de marché.
/// Récupéré par le telegram-bot auprès du BFF, sur réception du message de l'orchestrateur.
/// </summary>
public class MarketStressContext
{
    /// <summary>Indice Fear &amp; Greed (0-100)</summary>
    public int FearAndGreedIndex { get; set; }

    /// <summary>Classification textuelle (ex: "Extreme Fear", "Greed")</summary>
    public string FearAndGreedLabel { get; set; } = string.Empty;

    /// <summary>Nombre de positions ouvertes toutes paires confondues</summary>
    public int OpenPositionsCount { get; set; }

    /// <summary>P&amp;L du jour pour le quoteAsset concerné</summary>
    public double DailyPnl { get; set; }

    /// <summary>P&amp;L total pour le quoteAsset concerné</summary>
    public double TotalPnl { get; set; }
}
