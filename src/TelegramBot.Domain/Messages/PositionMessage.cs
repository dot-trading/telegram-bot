using System.Text;

namespace TelegramBot.Domain.Messages;

public record class PositionMessage(
    string Symbol,
    string Side,
    int? AiScore,
    double Entry,
    double Value,
    double Quantity,
    double StopLoss,
    double TakeProfile,
    DateTime CreatedAt)
{
    public override string ToString()
    {
        var messageBuilder = new StringBuilder();
        if (AiScore.HasValue) messageBuilder.Append($"\n<b>{Symbol}</b> — {Side} | Score IA: {AiScore}\n");
        else messageBuilder.Append($"\n<b>{Symbol}</b> — {Side}\n");

        // return $"\n<b>{Symbol}</b> — {Side} | Score IA: {r.AiScore}\n" +
        //        $"├─ Entrée: {r.Entry:F4}\n" +
        //        $"├─ Montant: {r.UsdtValue:F2}€ ({r.Quantity:F4})\n" +
        //        $"├─ P&amp;L: {pnlStr}\n" +
        //        $"├─ SL: {r.StopLoss:F4}{tp}\n" +
        //        $"└─ Depuis: {r.CreatedAt:dd/MM/yyyy HH:mm}";
        
        return messageBuilder.ToString();
    }
}