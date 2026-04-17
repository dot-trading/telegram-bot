using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Infrastructure.Persistence.Configurations;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> entity)
    {
        entity.ToTable("opportunities");
        entity.HasKey(o => o.Id);

        entity.Property(o => o.Id).HasColumnName("id");
        entity.Property(o => o.Symbol).HasColumnName("symbol").IsRequired();
        entity.Property(o => o.Score).HasColumnName("score");
        entity.Property(o => o.Signal).HasColumnName("signal").IsRequired();
        entity.Property(o => o.Reason).HasColumnName("reason").IsRequired();
        entity.Property(o => o.TargetPct).HasColumnName("target_pct");
        entity.Property(o => o.StopLossPct).HasColumnName("stop_loss_pct");
        entity.Property(o => o.Price).HasColumnName("price");
        entity.Property(o => o.Acted).HasColumnName("acted");
        entity.Property(o => o.CreatedAt).HasColumnName("created_at");
    }
}
