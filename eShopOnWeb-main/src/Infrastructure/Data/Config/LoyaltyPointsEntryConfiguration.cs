using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyPointsEntryConfiguration : IEntityTypeConfiguration<LoyaltyPointsEntry>
{
    public void Configure(EntityTypeBuilder<LoyaltyPointsEntry> builder)
    {
        builder.ToTable("LoyaltyPointsEntries");

        builder.HasKey(lpe => lpe.Id);

        builder.Property(lpe => lpe.Quantity)
            .IsRequired();

        builder.Property(lpe => lpe.SpentQuantity)
            .IsRequired();

        builder.Property(lpe => lpe.CreatedDate)
            .IsRequired();

        builder.Property(lpe => lpe.OrderId)
            .HasMaxLength(256);
    }
}
