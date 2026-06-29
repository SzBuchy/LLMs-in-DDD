using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Services
{
    public class ProductReviewPublicationPolicy : IProductReviewPublicationPolicy
    {
        private readonly IReviewRepository _reviewRepository;

        public ProductReviewPublicationPolicy(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> CanPublishAsync(Id<Customer> customerId, Id<Product> productId, CancellationToken cancellationToken = default)
        {
            var hasPublished = await _reviewRepository.HasPublishedReviewForProductAsync(customerId, productId, cancellationToken);
            return !hasPublished;
        }
    }
}
