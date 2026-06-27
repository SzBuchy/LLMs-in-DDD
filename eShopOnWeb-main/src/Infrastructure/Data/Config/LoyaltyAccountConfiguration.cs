using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.HasIndex(account => account.BuyerId)
            .IsUnique();

        builder.Property(account => account.BuyerId)
            .IsRequired()
            .HasMaxLength(256);

        var grantsNavigation = builder.Metadata.FindNavigation(nameof(LoyaltyAccount.PointGrants));
        grantsNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        var redemptionsNavigation = builder.Metadata.FindNavigation(nameof(LoyaltyAccount.Redemptions));
        redemptionsNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(account => account.PointGrants)
            .WithOne()
            .HasForeignKey(grant => grant.LoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(account => account.Redemptions)
            .WithOne()
            .HasForeignKey(redemption => redemption.LoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
