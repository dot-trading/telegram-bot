namespace TelegramBot.Domain.Abstractions;

public interface IBinanceService
{
    Task<Dictionary<string, double>> GetBalancesAsync();
    Task<double> GetCurrentPriceAsync(string symbol);
}
