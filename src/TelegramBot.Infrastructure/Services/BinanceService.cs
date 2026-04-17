using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Domain.Settings;

namespace TelegramBot.Infrastructure.Services;

public class BinanceService(IOptions<BinanceSettings> settings) : IBinanceService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };

    public async Task<Dictionary<string, double>> GetBalancesAsync()
    {
        var apiKey = settings.Value.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
            return [];

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var query = $"timestamp={timestamp}";
        var signature = Sign(query, settings.Value.ApiSecret);
        var url = $"{settings.Value.BaseUrl}/api/v3/account?{query}&signature={signature}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-MBX-APIKEY", apiKey);

        var response = await Http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("balances")
            .EnumerateArray()
            .Where(b => double.Parse(b.GetProperty("free").GetString()!) > 0 ||
                        double.Parse(b.GetProperty("locked").GetString()!) > 0)
            .ToDictionary(
                b => b.GetProperty("asset").GetString()!,
                b => double.Parse(b.GetProperty("free").GetString()!));
    }

    public async Task<double> GetCurrentPriceAsync(string symbol)
    {
        var url = $"{settings.Value.BaseUrl}/api/v3/ticker/price?symbol={symbol}";
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return double.Parse(doc.RootElement.GetProperty("price").GetString()!);
    }

    private static string Sign(string data, string secret)
    {
        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }
}
