using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class ProductReviewService : IProductReviewService
{
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IProductReviewRepository _reviewRepository;

    public ProductReviewService(IRepository<CatalogItem> itemRepository, IProductReviewRepository reviewRepository)
    {
        _itemRepository = itemRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<ProductReview> AddProductReviewAsync(string customerId, int catalogItemId, int rating, string textContent, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));
        Guard.Against.OutOfRange(catalogItemId, nameof(catalogItemId), 1, int.MaxValue);

        var product = await _itemRepository.GetByIdAsync(catalogItemId, cancellationToken);
        if (product == null)
        {
            throw new CatalogItemNotFoundException(catalogItemId);
        }

        var existingReviews = await _reviewRepository.GetByProductIdAsync(catalogItemId, cancellationToken);
        var hasPublishedReview = existingReviews.Any(r =>
            r.CustomerId == customerId &&
            r.Status == ReviewStatus.Published);

        if (hasPublishedReview)
        {
            throw new InvalidOperationException("Customer already has a published review for this product. The existing review must be withdrawn first.");
        }

        var newReview = new ProductReview(customerId, catalogItemId, rating, textContent);
        return await _reviewRepository.AddAsync(newReview, cancellationToken);
    }

    public async Task<ProductReview?> GetPublishedReviewByCustomerAndProductAsync(string customerId, int catalogItemId, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));
        Guard.Against.OutOfRange(catalogItemId, nameof(catalogItemId), 1, int.MaxValue);

        var reviews = await _reviewRepository.GetByProductIdAsync(catalogItemId, cancellationToken);
        return reviews.FirstOrDefault(r => r.CustomerId == customerId && r.Status == ReviewStatus.Published);
    }

    public async Task<ProductReview?> GetReviewByIdAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        Guard.Against.OutOfRange(reviewId, nameof(reviewId), 1, int.MaxValue);
        return await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
    }

    public async Task<ProductReview> EditProductReviewAsync(string customerId, int reviewId, int rating, string textContent, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));
        Guard.Against.OutOfRange(reviewId, nameof(reviewId), 1, int.MaxValue);

        var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review == null)
        {
            throw new ProductReviewNotFoundException(reviewId);
        }

        if (review.CustomerId != customerId)
        {
            throw new InvalidOperationException("You can only edit your own reviews.");
        }

        review.Edit(rating, textContent);
        await _reviewRepository.UpdateAsync(review, cancellationToken);
        return review;
    }
}
