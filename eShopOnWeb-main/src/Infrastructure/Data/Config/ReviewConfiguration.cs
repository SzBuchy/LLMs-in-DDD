using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasIndex(r => r.CatalogItemId);

        builder.Property(r => r.BuyerId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.CatalogItemId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(Review.MaxContentLength);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);
    }
}
