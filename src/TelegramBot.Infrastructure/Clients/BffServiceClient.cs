using System.Net.Http.Json;
using TelegramBot.Domain.Abstractions;

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
}
