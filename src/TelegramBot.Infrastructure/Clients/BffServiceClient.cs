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
                ? new PnlSummary(response.Daily, response.Weekly, response.Monthly, response.Total)
                : new PnlSummary(0, 0, 0, 0);
        }
        catch
        {
            return new PnlSummary(0, 0, 0, 0);
        }
    }

    private class PnlSummaryResponse
    {
        public double Daily { get; set; }
        public double Weekly { get; set; }
        public double Monthly { get; set; }
        public double Total { get; set; }
    }
}
