namespace VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.EditProductReview
{
    public record EditProductReviewCommand(
        Guid ReviewId,
        Guid CustomerId,
        Guid ProductId,
        int Rating,
        string Content) : ICommand<Guid>;
}
