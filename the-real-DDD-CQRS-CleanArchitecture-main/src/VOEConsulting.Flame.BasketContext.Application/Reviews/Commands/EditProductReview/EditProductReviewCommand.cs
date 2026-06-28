namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditProductReview
{
    public record EditProductReviewCommand(Guid ReviewId, int Rating, string Content) : ICommand<Guid>;
}
