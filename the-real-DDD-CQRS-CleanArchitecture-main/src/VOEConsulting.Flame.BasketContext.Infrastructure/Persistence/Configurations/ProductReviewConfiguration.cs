using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReviewEntity>
{
    public void Configure(EntityTypeBuilder<ProductReviewEntity> builder)
    {
        builder.ToTable("ProductReviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CustomerId)
            .IsRequired();

        builder.Property(r => r.ProductId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => new { r.CustomerId, r.ProductId });
    }
}
