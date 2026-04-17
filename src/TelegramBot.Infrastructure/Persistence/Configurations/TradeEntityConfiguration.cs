using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Infrastructure.Persistence.Configurations;

public class TradeEntityConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> entity)
    {
        entity.ToTable("trades");
        entity.HasKey(t => t.Id);

        entity.Property(t => t.Id).HasColumnName("id");
        entity.Property(t => t.Symbol).HasColumnName("symbol").IsRequired();
        entity.Property(t => t.Side).HasColumnName("side").IsRequired();
        entity.Property(t => t.Status).HasColumnName("status").IsRequired();
        entity.Property(t => t.Price).HasColumnName("price");
        entity.Property(t => t.Quantity).HasColumnName("quantity");
        entity.Property(t => t.UsdtValue).HasColumnName("usdt_value");
        entity.Property(t => t.StopLoss).HasColumnName("stop_loss");
        entity.Property(t => t.TakeProfit).HasColumnName("take_profit");
        entity.Property(t => t.AiScore).HasColumnName("ai_score");
        entity.Property(t => t.ClosePrice).HasColumnName("close_price");
        entity.Property(t => t.PnlUsdt).HasColumnName("pnl_usdt");
        entity.Property(t => t.PnlPct).HasColumnName("pnl_pct");
        entity.Property(t => t.CreatedAt).HasColumnName("created_at");
        entity.Property(t => t.CloseAt).HasColumnName("close_at");
    }
}
