using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReviewEntity>
    {
        public void Configure(EntityTypeBuilder<ProductReviewEntity> builder)
        {
            builder.ToTable("ProductReviews");

            builder.HasKey(review => review.Id);

            builder.Property(review => review.CustomerId)
                .IsRequired();

            builder.Property(review => review.ProductId)
                .IsRequired();

            builder.Property(review => review.Rating)
                .IsRequired();

            builder.Property(review => review.Content)
                .HasMaxLength(ProductReview.MaxContentLength)
                .IsRequired();

            builder.Property(review => review.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(review => review.ProductId);
        }
    }
}
