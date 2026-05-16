using TelegramBot.Domain.Messages;

namespace TelegramBot.Domain.Abstractions;

/// <summary>
/// Abstraction for querying the BFF (Backend For Frontend) API.
/// Only exposes endpoints that are still consumed via the legacy wrapper;
/// new notification endpoints are consumed directly through <c>IBffApiClient</c>
/// from the <c>TradingProject.Bff.Client</c> package.
/// </summary>
public interface IBffService
{
    /// <summary>
    /// Retrieves the P&amp;L summary for a given spot (or all spots).
    /// </summary>
    Task<PnlSummary> GetPnlSummaryAsync(string? spot = null, CancellationToken ct = default);
}
