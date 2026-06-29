using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.ToTable("LoyaltyAccounts");

        builder.HasKey(la => la.Id);

        builder.Property(la => la.CustomerId)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(la => la.CustomerId)
            .IsUnique();

        var entriesNavigation = builder.Metadata.FindNavigation(nameof(LoyaltyAccount.Entries));
        entriesNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        var transactionsNavigation = builder.Metadata.FindNavigation(nameof(LoyaltyAccount.Transactions));
        transactionsNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
