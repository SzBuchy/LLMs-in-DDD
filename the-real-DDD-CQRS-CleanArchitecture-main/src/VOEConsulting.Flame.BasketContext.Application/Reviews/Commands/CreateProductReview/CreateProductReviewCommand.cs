namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.CreateProductReview
{
    public record CreateProductReviewCommand(Guid CustomerId, Guid ProductId, int Rating, string Content) : ICommand<Guid>;
}
