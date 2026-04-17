namespace TelegramBot.Domain.Abstractions;

public interface IMessageFormatter
{
    Task<string> GetStatusMessageAsync();
    string GetStatsMessage();
    Task<string> GetPnlMessageAsync();
    Task<string> GetPositionsMessageAsync();
    string GetTradesMessage(int limit = 5);
    Task<string> GetAgentsMessageAsync();
    string GetClusterMessage();
}
