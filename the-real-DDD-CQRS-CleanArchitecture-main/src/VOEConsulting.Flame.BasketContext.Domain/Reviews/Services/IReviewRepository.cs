using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Services
{
    public interface IReviewRepository
    {
        Task<bool> HasPublishedReviewForProductAsync(Id<Customer> customerId, Id<Product> productId, CancellationToken cancellationToken = default);
        Task AddAsync(Review review, CancellationToken cancellationToken = default);
        Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
        Task<Review?> GetByIdAsync(Id<Review> id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Review>> GetByProductIdAsync(Id<Product> productId, CancellationToken cancellationToken = default);
    }
}
