using System.Net.Http.Json;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Domain.Messages;

namespace TelegramBot.Infrastructure.Clients;

public class BffServiceClient(HttpClient httpClient) : IBffService
{
    private const string BaseUrl = "http://trading-bff-api/api";

    public async Task<PnlSummary> GetPnlSummaryAsync(string? spot = null, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/pnl/summary";
        if (!string.IsNullOrEmpty(spot))
        {
            url += $"?quoteAsset={spot.ToUpper()}";
        }

        try
        {
            var response = await httpClient.GetFromJsonAsync<PnlSummaryResponse>(url, ct);
            return response != null 
                ? new PnlSummary(response.Today.Value, response.ThisWeek.Value, response.ThisMonth.Value, response.Total.Value)
                : new PnlSummary(0, 0, 0, 0);
        }
        catch
        {
            return new PnlSummary(0, 0, 0, 0);
        }
    }

    public async Task<MarketStressContext> GetMarketStressContextAsync(string quoteAsset, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/notifications/market-stress-context?quoteAsset={quoteAsset.ToUpper()}";

        try
        {
            var response = await httpClient.GetFromJsonAsync<MarketStressContextResponse>(url, ct);
            if (response is null)
                return new MarketStressContext();

            return new MarketStressContext
            {
                FearAndGreedIndex  = response.FearAndGreedIndex,
                FearAndGreedLabel  = response.FearAndGreedLabel,
                OpenPositionsCount = response.OpenPositionsCount,
                DailyPnl           = response.DailyPnl,
                TotalPnl           = response.TotalPnl,
            };
        }
        catch
        {
            // En cas d'erreur BFF, on retourne un contexte vide plutôt que de bloquer la notification
            return new MarketStressContext();
        }
    }

    // ── DTOs de désérialisation ───────────────────────────────────────────────

    private class PnlSummaryResponse
    {
        public PnlSummaryItemResponse Today { get; set; } = new();
        public PnlSummaryItemResponse ThisWeek { get; set; } = new();
        public PnlSummaryItemResponse ThisMonth { get; set; } = new();
        public PnlSummaryItemResponse Total { get; set; } = new();
    }

    private class PnlSummaryItemResponse
    {
        public double Value { get; set; }
    }

    private class MarketStressContextResponse
    {
        public int    FearAndGreedIndex  { get; set; }
        public string FearAndGreedLabel  { get; set; } = string.Empty;
        public int    OpenPositionsCount { get; set; }
        public double DailyPnl           { get; set; }
        public double TotalPnl           { get; set; }
    }
}
