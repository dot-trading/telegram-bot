namespace TelegramBot.Domain.Constants;

public static class MessageTopic
{
    public const string ReportTopicPath       = "commands/report";
    public const string MarketStressTopicPath = "notification/market-stress";
    public const string ServiceStartTopicPath = "notification/service-start";
    public const string AlertsTopicPath       = "trading/alerts";
}