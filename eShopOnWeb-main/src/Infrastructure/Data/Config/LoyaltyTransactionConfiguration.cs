using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.ToTable("LoyaltyTransactions");

        builder.HasKey(lt => lt.Id);

        builder.Property(lt => lt.Amount)
            .IsRequired();

        builder.Property(lt => lt.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(lt => lt.CreatedDate)
            .IsRequired();

        builder.Property(lt => lt.OrderId)
            .HasMaxLength(256);
    }
}
