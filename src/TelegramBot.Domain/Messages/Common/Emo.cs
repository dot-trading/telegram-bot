namespace TelegramBot.Domain.Messages.Common;

public class Emo(double balance) 
{
    private double Balance => balance;
    public override string ToString()
    {
        return Balance >= 0 ? "📈" : "📉";
    }
}