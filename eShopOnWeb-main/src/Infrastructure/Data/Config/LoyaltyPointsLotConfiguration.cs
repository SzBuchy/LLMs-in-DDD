using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyPointsLotConfiguration : IEntityTypeConfiguration<LoyaltyPointsLot>
{
    public void Configure(EntityTypeBuilder<LoyaltyPointsLot> builder)
    {
        builder.ToTable("LoyaltyPointsLots");

        builder.Property(l => l.Points).IsRequired();
        builder.Property(l => l.RemainingPoints).IsRequired();
        builder.Property(l => l.EarnedDate).IsRequired();
        builder.Property(l => l.ExpirationDate).IsRequired();
    }
}
