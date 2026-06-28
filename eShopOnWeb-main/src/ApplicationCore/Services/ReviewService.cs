using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IRepository<CatalogItem> _catalogItemRepository;

    public ReviewService(IReviewRepository reviewRepository, IRepository<CatalogItem> catalogItemRepository)
    {
        _reviewRepository = reviewRepository;
        _catalogItemRepository = catalogItemRepository;
    }

    public async Task<Review> AddReviewAsync(string buyerId, int catalogItemId, int rating, string content)
    {
        var catalogItem = await _catalogItemRepository.GetByIdAsync(catalogItemId);
        if (catalogItem is null)
        {
            throw new CatalogItemNotFoundException(catalogItemId);
        }

        var review = new Review(buyerId, catalogItemId, rating, content);

        return await _reviewRepository.AddAsync(review);
    }

    public async Task<Review> EditReviewAsync(string buyerId, int reviewId, int rating, string content)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);

        // A review that does not belong to this buyer is treated the same as a missing
        // one, so callers cannot probe for the existence of other customers' reviews.
        if (review is null || review.BuyerId != buyerId)
        {
            throw new ReviewNotFoundException(reviewId);
        }

        review.Edit(rating, content);

        await _reviewRepository.UpdateAsync(review);

        return review;
    }
}
