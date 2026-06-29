using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.Infrastructure.Data.Config;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");

        builder.Property(pr => pr.Id)
            .IsRequired();

        builder.Property(pr => pr.CustomerId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pr => pr.CatalogItemId)
            .IsRequired();

        builder.Property(pr => pr.Rating)
            .IsRequired();

        builder.Property(pr => pr.TextContent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pr => pr.Status)
            .IsRequired()
            .HasConversion<int>();
    }
}
