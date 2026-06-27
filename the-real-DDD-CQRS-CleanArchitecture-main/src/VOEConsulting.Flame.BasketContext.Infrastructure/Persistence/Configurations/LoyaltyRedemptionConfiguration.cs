using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Configurations
{
    public class LoyaltyRedemptionConfiguration : IEntityTypeConfiguration<LoyaltyRedemptionEntity>
    {
        public void Configure(EntityTypeBuilder<LoyaltyRedemptionEntity> builder)
        {
            builder.ToTable("LoyaltyRedemptions");

            builder.HasKey(redemption => redemption.Id);

            builder.Property(redemption => redemption.OrderId)
                .IsRequired();

            builder.Property(redemption => redemption.Points)
                .IsRequired();

            builder.Property(redemption => redemption.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(redemption => redemption.RedeemedAtUtc)
                .IsRequired();

            builder.HasIndex(redemption => new { redemption.LoyaltyAccountId, redemption.OrderId })
                .IsUnique();
        }
    }
}
