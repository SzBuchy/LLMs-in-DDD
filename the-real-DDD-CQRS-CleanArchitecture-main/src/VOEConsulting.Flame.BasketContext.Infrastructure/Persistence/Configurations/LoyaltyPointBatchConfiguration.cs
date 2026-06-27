using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Configurations
{
    public class LoyaltyPointBatchConfiguration : IEntityTypeConfiguration<LoyaltyPointBatchEntity>
    {
        public void Configure(EntityTypeBuilder<LoyaltyPointBatchEntity> builder)
        {
            builder.ToTable("LoyaltyPointBatches");

            builder.HasKey(batch => batch.Id);

            builder.Property(batch => batch.OrderId)
                .IsRequired();

            builder.Property(batch => batch.Points)
                .IsRequired();

            builder.Property(batch => batch.RedeemedPoints)
                .IsRequired();

            builder.Property(batch => batch.AwardedAtUtc)
                .IsRequired();

            builder.Property(batch => batch.ExpiresAtUtc)
                .IsRequired();

            builder.HasIndex(batch => new { batch.LoyaltyAccountId, batch.OrderId })
                .IsUnique();
        }
    }
}
