namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class ProductReviewEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
