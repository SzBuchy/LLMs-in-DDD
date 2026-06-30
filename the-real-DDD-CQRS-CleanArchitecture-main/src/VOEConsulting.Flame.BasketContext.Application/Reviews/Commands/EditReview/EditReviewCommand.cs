using System;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditReview
{
    public record EditReviewCommand(
        Guid ReviewId,
        Guid CustomerId,
        int Rating,
        string Content
    ) : ICommand<Guid>;
}
