namespace TelegramBot.Domain.Messages.Common;

public class BrokerMessagePayload
{
    public string MessageType { get; set; }
        = string.Empty;
    
    public IDictionary<string, string> MessageArgs { get; set; }
        = new  Dictionary<string, string>();
}