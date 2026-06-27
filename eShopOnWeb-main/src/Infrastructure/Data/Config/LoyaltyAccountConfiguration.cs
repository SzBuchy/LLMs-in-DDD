using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.ToTable("LoyaltyAccounts");

        var navigation = builder.Metadata.FindNavigation(nameof(LoyaltyAccount.PointsLots));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(a => a.BuyerId)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(a => a.BuyerId)
            .IsUnique();

        builder.HasMany(a => a.PointsLots)
            .WithOne()
            .HasForeignKey("LoyaltyAccountId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
