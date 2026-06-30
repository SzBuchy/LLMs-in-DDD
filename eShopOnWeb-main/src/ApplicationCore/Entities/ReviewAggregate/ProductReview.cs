using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

public class ProductReview : BaseEntity, IAggregateRoot
{
    public string CustomerId { get; private set; }
    public int CatalogItemId { get; private set; }
    public int Rating { get; private set; }
    public string TextContent { get; private set; }
    public ReviewStatus Status { get; private set; }

    #pragma warning disable CS8618 // Required by Entity Framework
    private ProductReview() {}

    public ProductReview(string customerId, int catalogItemId, int rating, string textContent)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));
        Guard.Against.OutOfRange(catalogItemId, nameof(catalogItemId), 1, int.MaxValue);
        Guard.Against.OutOfRange(rating, nameof(rating), 1, 5);
        Guard.Against.NullOrEmpty(textContent, nameof(textContent));
        
        if (textContent.Length < 10 || textContent.Length > 500)
        {
            throw new ArgumentException("Text content length must be between 10 and 500 characters.", nameof(textContent));
        }

        CustomerId = customerId;
        CatalogItemId = catalogItemId;
        Rating = rating;
        TextContent = textContent;
        Status = ReviewStatus.PendingModeration;
    }

    public void Approve()
    {
        if (Status == ReviewStatus.Published)
        {
            return;
        }

        Status = ReviewStatus.Published;
    }

    public void Reject()
    {
        if (Status == ReviewStatus.Rejected)
        {
            return;
        }

        Status = ReviewStatus.Rejected;
    }

    public void SendToModeration()
    {
        if (Status == ReviewStatus.PendingModeration)
        {
            return;
        }

        Status = ReviewStatus.PendingModeration;
    }

    public void Publish(IProductReviewPublicationPolicy policy, IEnumerable<ProductReview> existingReviews)
    {
        Guard.Against.Null(policy, nameof(policy));
        Guard.Against.Null(existingReviews, nameof(existingReviews));

        policy.Validate(this, existingReviews);
        Status = ReviewStatus.Published;
    }

    public void Withdraw()
    {
        if (Status == ReviewStatus.Withdrawn)
        {
            return;
        }

        Status = ReviewStatus.Withdrawn;
    }

    public void Edit(int rating, string textContent)
    {
        if (Status != ReviewStatus.Published)
        {
            throw new InvalidOperationException("Only published reviews can be edited.");
        }

        Guard.Against.OutOfRange(rating, nameof(rating), 1, 5);
        Guard.Against.NullOrEmpty(textContent, nameof(textContent));
        
        if (textContent.Length < 10 || textContent.Length > 500)
        {
            throw new ArgumentException("Text content length must be between 10 and 500 characters.", nameof(textContent));
        }

        Rating = rating;
        TextContent = textContent;
        Status = ReviewStatus.PendingModeration;
    }
}
