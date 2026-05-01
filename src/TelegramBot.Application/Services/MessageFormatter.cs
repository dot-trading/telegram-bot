using TelegramBot.Domain.Abstractions;
using TelegramBot.Domain.Constants;

namespace TelegramBot.Application.Services;

public class MessageFormatter(IDatabaseService db, IBinanceService binance, IClusterService cluster)
    : IMessageFormatter
{
    private static string Now() => DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    public async Task<string> GetStatusMessageAsync()
    {
        try
        {
            var bals = await binance.GetBalancesAsync();
            var total = bals.GetValueOrDefault("EUR") + bals.GetValueOrDefault("USDC") + bals.GetValueOrDefault("USDT");
            var openCount = db.GetOpenPositionsCount();
            var dailyPnl = db.GetDailyPnl();
            var totalPnl = db.GetTotalPnl();
            var orchStatus = cluster.GetOrchestratorStatus();
            var ollamaStatus = await cluster.GetOllamaStatusAsync();
            var persistenceStatus = await cluster.GetPersistenceStatusAsync();
            var emo = dailyPnl >= 0 ? "📈" : "📉";

            return $"{IconsStrings.Robot} <b>TRADING AI — STATUS</b>\n" +
                   $"━━━━━━━━━━━━━━━━━━━━\n" +
                   $"{IconsStrings.System} Système: <b>ONLINE</b>\n\n" +
                   $"{IconsStrings.Spanner} <b>Infrastructure:</b>\n" +
                   $"├─ Orchestrateur: {orchStatus}\n" +
                   $"├─ Ollama (IA): {ollamaStatus}\n" +
                   $"├─ Persistence: {persistenceStatus}\n" +
                   $"└─ Database: {IconsStrings.Ok} Connected\n\n" +
                   $"{IconsStrings.CashBag} <b>Portfolio:</b>\n" +
                   $"├─ Capital libre: {total:F2}€\n" +
                   $"│   ├─ EUR: {bals.GetValueOrDefault("EUR"):F2}\n" +
                   $"│   ├─ USDC: {bals.GetValueOrDefault("USDC"):F2}\n" +
                   $"│   └─ BTC: {bals.GetValueOrDefault("BTC"):F6}\n" +
                   $"├─ Positions ouvertes: {openCount}\n" +
                   $"├─ {emo} P&amp;L jour: {dailyPnl:+0.00;-0.00}€\n" +
                   $"└─ P&amp;L total: {totalPnl:+0.00;-0.00}€\n\n" +
                   $"⚡ {Now()}";
        }
        catch (Exception e) { return $"{IconsStrings.Ko} Erreur status: {e.Message}"; }
    }

    public string GetStatsMessage()
    {
        try
        {
            var s = db.GetStats();
            return $"📊 <b>STATISTIQUES</b> — {DateTime.Now:dd/MM/yyyy}\n" +
                   $"━━━━━━━━━━━━━━━━━━━━\n" +
                   $"📈 <b>Performance:</b>\n" +
                   $"├─ Aujourd'hui: {s.PnlDay:+0.00;-0.00}€ ({s.CountDay} trades)\n" +
                   $"├─ Cette semaine: {s.PnlWeek:+0.00;-0.00}€ ({s.CountWeek} trades)\n" +
                   $"├─ Ce mois: {s.PnlMonth:+0.00;-0.00}€ ({s.CountMonth} trades)\n" +
                   $"└─ Total: {s.PnlTotal:+0.00;-0.00}€ ({s.CountTotal} trades)\n\n" +
                   $"🎯 <b>Métriques:</b>\n" +
                   $"├─ Win Rate: {s.WinRate:F1}% ({s.Wins}/{s.CountTotal})\n" +
                   $"└─ Trades fermés: {s.CountTotal}\n\n" +
                   $"⚡ {Now()}";
        }
        catch (Exception e) { return $"❌ Erreur stats: {e.Message}"; }
    }

    public async Task<string> GetPnlMessageAsync()
    {
        try
        {
            var s = db.GetPnlSummary();
            var bals = await binance.GetBalancesAsync();
            var totalBal = bals.GetValueOrDefault("EUR") + bals.GetValueOrDefault("USDC") + bals.GetValueOrDefault("USDT");

            static string Emo(double v) => v >= 0 ? "📈" : "📉";
            return $"💰 <b>PNL ACTUEL</b>\n" +
                   $"━━━━━━━━━━━━━━━━━━━━\n" +
                   $"💵 Capital libre : {totalBal:F2} USDT\n\n" +
                   $"📊 <b>Rendements Nets :</b>\n" +
                   $"{Emo(s.Daily)} Aujourd'hui : {s.Daily:+0.00;-0.00}€\n" +
                   $"{Emo(s.Weekly)} Cette semaine : {s.Weekly:+0.00;-0.00}€\n" +
                   $"{Emo(s.Monthly)} Ce mois : {s.Monthly:+0.00;-0.00}€\n" +
                   $"{Emo(s.Total)} <b>Total : {s.Total:+0.00;-0.00}€</b>\n\n" +
                   $"⚡ {Now()}";
        }
        catch (Exception e) { return $"❌ Erreur PNL: {e.Message}"; }
    }

    public async Task<string> GetPositionsMessageAsync()
    {
        try
        {
            var rows = db.GetOpenPositions();
            if (rows.Count == 0)
                return $"📂 <b>POSITIONS OUVERTES</b>\n━━━━━━━━━━━━━━━━━━━━\nAucune position ouverte.\n\n⚡ {Now()}";

            var lines = new List<string> { $"📂 <b>POSITIONS OUVERTES ({rows.Count})</b>\n━━━━━━━━━━━━━━━━━━━━" };
            foreach (var r in rows)
            {
                string pnlStr;
                try
                {
                    var cur = await binance.GetCurrentPriceAsync(r.Symbol);
                    var pnlPct = (cur - r.Entry) / r.Entry * 100;
                    var pnlEur = (cur - r.Entry) * r.Quantity;
                    var emo = pnlPct >= 0 ? "🟢" : "🔴";
                    pnlStr = $"{emo} {pnlPct:+0.00;-0.00}% ({pnlEur:+0.00;-0.00}€)";
                }
                catch { pnlStr = "N/A"; }

                var tp = r.TakeProfit.HasValue ? $" | TP: {r.TakeProfit:F4}" : "";
                lines.Add(
                    $"\n<b>{r.Symbol}</b> — {r.Side} | Score IA: {r.AiScore}\n" +
                    $"├─ Entrée: {r.Entry:F4}\n" +
                    $"├─ Montant: {r.UsdtValue:F2}€ ({r.Quantity:F4})\n" +
                    $"├─ P&amp;L: {pnlStr}\n" +
                    $"├─ SL: {r.StopLoss:F4}{tp}\n" +
                    $"└─ Depuis: {r.CreatedAt:dd/MM/yyyy HH:mm}");
            }
            lines.Add($"\n⚡ {Now()}");
            return string.Join("\n", lines);
        }
        catch (Exception e) { return $"❌ Erreur positions: {e.Message}"; }
    }

    public string GetTradesMessage(int limit = 5)
    {
        try
        {
            var rows = db.GetLastTrades(limit);
            if (rows.Count == 0)
                return $"📜 <b>DERNIERS TRADES</b>\n━━━━━━━━━━━━━━━━━━━━\nAucun trade fermé.\n\n⚡ {Now()}";

            var lines = new List<string> { $"📜 <b>DERNIERS TRADES ({rows.Count})</b>\n━━━━━━━━━━━━━━━━━━━━" };
            foreach (var r in rows)
            {
                var emo = r.Pnl >= 0 ? "✅" : "❌";
                lines.Add(
                    $"\n{emo} <b>{r.Symbol}</b>\n" +
                    $"├─ Entrée: {r.Entry:F4} → Sortie: {r.ClosePrice:F4}\n" +
                    $"├─ P&amp;L: {r.Pnl:+0.00;-0.00}€ ({r.PnlPct:+0.00;-0.00}%)\n" +
                    $"└─ Fermé: {r.ClosedAt:dd/MM/yyyy HH:mm}");
            }
            lines.Add($"\n⚡ {Now()}");
            return string.Join("\n", lines);
        }
        catch (Exception e) { return $"❌ Erreur trades: {e.Message}"; }
    }

    public async Task<string> GetAgentsMessageAsync()
    {
        try
        {
            var models = await cluster.GetOllamaModelsAsync();
            var lines = new List<string> { "🤖 <b>AGENTS IA (Ollama)</b>\n━━━━━━━━━━━━━━━━━━━━" };
            if (models.Count == 0)
                lines.Add("Aucun modèle chargé.");
            foreach (var m in models)
            {
                var sizeGb = m.Size / 1e9;
                var date = m.ModifiedAt.Length >= 10 ? m.ModifiedAt[..10] : m.ModifiedAt;
                lines.Add($"✅ <b>{m.Name}</b>\n   └─ {sizeGb:F1} GB | Chargé: {date}");
            }
            lines.Add($"\n⚡ {Now()}");
            return string.Join("\n", lines);
        }
        catch (Exception e) { return $"❌ Ollama non accessible: {e.Message}"; }
    }

    public string GetClusterMessage()
    {
        try
        {
            var raw = cluster.GetK8sPodsStatus();
            var lines = new List<string> { "☸️ <b>KUBERNETES — trading-ai</b>\n━━━━━━━━━━━━━━━━━━━━" };
            foreach (var line in raw.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var name   = parts.Length > 0 ? parts[0] : "";
                var status = parts.Length > 1 ? parts[1] : "?";
                var node   = parts.Length > 2 ? parts[2] : "?";
                var emo    = status == "Running" ? "✅" : "❌";
                lines.Add($"{emo} <code>{name[..Math.Min(35, name.Length)]}</code>\n   └─ {status} | {node}");
            }
            lines.Add($"\n⚡ {Now()}");
            return string.Join("\n", lines);
        }
        catch (Exception e) { return $"❌ Erreur Cluster: {e.Message}"; }
    }
}
