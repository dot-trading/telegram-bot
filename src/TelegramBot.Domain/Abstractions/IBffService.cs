namespace TelegramBot.Domain.Abstractions;

public interface IBffService
{
    Task<PnlSummary> GetPnlSummaryAsync(string? spot = null, CancellationToken ct = default);
}
