using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class ReviewService : IReviewService
{
    private readonly IReadRepository<CatalogItem> _catalogItemRepository;
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(
        IReadRepository<CatalogItem> catalogItemRepository,
        IReviewRepository reviewRepository)
    {
        _catalogItemRepository = catalogItemRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<Review> AddReviewAsync(
        string buyerId,
        int catalogItemId,
        int rating,
        string content,
        CancellationToken cancellationToken = default)
    {
        var catalogItem = await _catalogItemRepository.GetByIdAsync(catalogItemId, cancellationToken);
        if (catalogItem is null)
        {
            throw new CatalogItemNotFoundException(catalogItemId);
        }

        var review = new Review(buyerId, catalogItemId, rating, content);

        return await _reviewRepository.AddAsync(review, cancellationToken);
    }
}
