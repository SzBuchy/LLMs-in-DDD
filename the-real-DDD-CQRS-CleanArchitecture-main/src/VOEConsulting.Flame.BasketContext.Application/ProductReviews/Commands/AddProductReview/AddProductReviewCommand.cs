namespace VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.AddProductReview
{
    public record AddProductReviewCommand(
        Guid CustomerId,
        Guid ProductId,
        int Rating,
        string Content) : ICommand<Guid>;
}
