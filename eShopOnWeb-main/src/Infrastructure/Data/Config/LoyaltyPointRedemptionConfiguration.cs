using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyPointRedemptionConfiguration : IEntityTypeConfiguration<LoyaltyPointRedemption>
{
    public void Configure(EntityTypeBuilder<LoyaltyPointRedemption> builder)
    {
        builder.Property(redemption => redemption.PointsRedeemed)
            .IsRequired();

        builder.Property(redemption => redemption.DiscountAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(redemption => redemption.RedeemedAt)
            .IsRequired();
    }
}
