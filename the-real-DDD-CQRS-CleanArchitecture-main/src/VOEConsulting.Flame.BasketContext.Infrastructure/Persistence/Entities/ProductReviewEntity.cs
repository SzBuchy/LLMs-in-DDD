using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class ProductReviewEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; } = string.Empty;
        public ProductReviewStatus Status { get; set; }
    }
}
