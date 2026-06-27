using VOEConsulting.Flame.BasketContext.Domain.Reviews;

namespace VOEConsulting.Flame.BasketContext.Application.Repositories
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId);
    }
}
