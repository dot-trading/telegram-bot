using System.Text;
using TelegramBot.Domain.Messages.Common;

namespace TelegramBot.Domain.Messages;

public class PnlMessage(
    double daily,
    double weekly,
    double monthly,
    IDictionary<string, double> capital)
{
    public override string ToString()
    {
        var message = new StringBuilder();
        message.Append("💰 <b>CURRENT PNL</b>\n");
        message.Append("━━━━━━━━━━━━━━━━━━━━\n");

        foreach (var kvp in capital)
        {
            message.Append($"💵 Capital libre : {kvp.Value:F2} {kvp.Key}\n\n");
        }
        
        message.Append("📊 <b>Rendements Nets :</b>\n");
        
        message.Append($"{new Emo(daily)} Aujourd'hui : {daily:+0.00;-0.00}€\n");
        message.Append($"{new Emo(weekly)} Cette semaine : {weekly:+0.00;-0.00}€\n");
        message.Append($"{new Emo(monthly)} Ce mois : {monthly:+0.00;-0.00}€\n");
        
        // return $"💰 <b>PNL ACTUEL</b>\n" +
        //        $"━━━━━━━━━━━━━━━━━━━━\n" +
        //        $"💵 Capital libre : {capital:F2} USDT\n\n" +
        //        $"📊 <b>Rendements Nets :</b>\n" +
        //        $"{new Emo(daily)} Aujourd'hui : {daily:+0.00;-0.00}€\n" +
        //        $"{new Emo(weekly)} Cette semaine : {weekly:+0.00;-0.00}€\n" +
        //        $"{new Emo(monthly)} Ce mois : {monthly:+0.00;-0.00}€\n";

        return message.ToString();
    }
}