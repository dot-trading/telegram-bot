namespace TelegramBot.Domain.Entities;

public class PortfolioSnapshot
{
    public int Id { get; set; }
    public double TotalUsdt { get; set; }
    public double FreeUsdt { get; set; }
    public int PositionsCount { get; set; }
    public double DailyPnl { get; set; }
    public double TotalPnl { get; set; }
    public DateTime CreatedAt { get; set; }
}
