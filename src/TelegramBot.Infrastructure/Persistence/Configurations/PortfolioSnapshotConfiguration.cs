using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Infrastructure.Persistence.Configurations;

public class PortfolioSnapshotConfiguration : IEntityTypeConfiguration<PortfolioSnapshot>
{
    public void Configure(EntityTypeBuilder<PortfolioSnapshot> entity)
    {
        entity.ToTable("portfolio_snapshots");
        entity.HasKey(s => s.Id);

        entity.Property(s => s.Id).HasColumnName("id");
        entity.Property(s => s.TotalUsdt).HasColumnName("total_usdt");
        entity.Property(s => s.FreeUsdt).HasColumnName("free_usdt");
        entity.Property(s => s.PositionsCount).HasColumnName("positions_count");
        entity.Property(s => s.DailyPnl).HasColumnName("daily_pnl");
        entity.Property(s => s.TotalPnl).HasColumnName("total_pnl");
        entity.Property(s => s.CreatedAt).HasColumnName("created_at");
    }
}
