namespace TelegramBot.Domain.Entities;

public class Opportunity
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Signal { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public double? TargetPct { get; set; }
    public double? StopLossPct { get; set; }
    public double Price { get; set; }
    public bool Acted { get; set; }
    public DateTime CreatedAt { get; set; }
}
