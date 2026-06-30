using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IProductReviewService
{
    Task<ProductReview> AddProductReviewAsync(string customerId, int catalogItemId, int rating, string textContent, CancellationToken cancellationToken = default);
    Task<ProductReview?> GetPublishedReviewByCustomerAndProductAsync(string customerId, int catalogItemId, CancellationToken cancellationToken = default);
    Task<ProductReview?> GetReviewByIdAsync(int reviewId, CancellationToken cancellationToken = default);
    Task<ProductReview> EditProductReviewAsync(string customerId, int reviewId, int rating, string textContent, CancellationToken cancellationToken = default);
}
