using System;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.AddReview
{
    public record AddReviewCommand(
        Guid CustomerId,
        Guid ProductId,
        int Rating,
        string Content
    ) : ICommand<Guid>;
}
