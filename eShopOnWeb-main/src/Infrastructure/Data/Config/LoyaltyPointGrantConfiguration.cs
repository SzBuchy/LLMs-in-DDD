using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyPointGrantConfiguration : IEntityTypeConfiguration<LoyaltyPointGrant>
{
    public void Configure(EntityTypeBuilder<LoyaltyPointGrant> builder)
    {
        builder.HasIndex(grant => grant.SourceOrderId)
            .IsUnique()
            .HasFilter("[SourceOrderId] IS NOT NULL");

        builder.Property(grant => grant.PointsAwarded)
            .IsRequired();

        builder.Property(grant => grant.PointsRemaining)
            .IsRequired();

        builder.Property(grant => grant.AwardedAt)
            .IsRequired();

        builder.Property(grant => grant.ExpiresAt)
            .IsRequired();
    }
}
