using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

public class Review : BaseEntity, IAggregateRoot
{
    public const int MinContentLength = 10;
    public const int MaxContentLength = 500;
    public const int MinRating = 1;
    public const int MaxRating = 5;

    public string BuyerId { get; private set; }
    public int CatalogItemId { get; private set; }
    public int Rating { get; private set; }
    public string Content { get; private set; }
    public ReviewStatus Status { get; private set; }

    #pragma warning disable CS8618 // Required by Entity Framework
    private Review() { }

    public Review(string buyerId, int catalogItemId, int rating, string content)
    {
        Guard.Against.NullOrWhiteSpace(buyerId, nameof(buyerId));
        Guard.Against.NegativeOrZero(catalogItemId, nameof(catalogItemId));
        Guard.Against.OutOfRange(rating, nameof(rating), MinRating, MaxRating);
        GuardAgainstInvalidContent(content);

        BuyerId = buyerId;
        CatalogItemId = catalogItemId;
        Rating = rating;
        Content = content;
        Status = ReviewStatus.PendingModeration;
    }

    public void Publish(IEnumerable<Review> existingReviews)
    {
        Publish(MaximumOnePublishedReviewPerProductPolicy.Instance, existingReviews);
    }

    public void Publish(IReviewPublicationPolicy publicationPolicy, IEnumerable<Review> existingReviews)
    {
        GuardAgainstNonPendingStatus(nameof(Publish));
        Guard.Against.Null(publicationPolicy, nameof(publicationPolicy));

        publicationPolicy.EnsureCanPublish(this, existingReviews);

        Status = ReviewStatus.Published;
    }

    public void Reject()
    {
        GuardAgainstNonPendingStatus(nameof(Reject));

        Status = ReviewStatus.Rejected;
    }

    public void Withdraw()
    {
        if (Status != ReviewStatus.Published)
        {
            throw new InvalidOperationException(
                $"Cannot {nameof(Withdraw)} a review with status '{Status}'. Only '{ReviewStatus.Published}' reviews can be withdrawn.");
        }

        Status = ReviewStatus.Withdrawn;
    }

    private void GuardAgainstNonPendingStatus(string transitionName)
    {
        if (Status != ReviewStatus.PendingModeration)
        {
            throw new InvalidOperationException(
                $"Cannot {transitionName} a review with status '{Status}'. Only '{ReviewStatus.PendingModeration}' reviews can change moderation status.");
        }
    }

    private static void GuardAgainstInvalidContent(string content)
    {
        Guard.Against.NullOrWhiteSpace(content, nameof(content));
        if (content.Length < MinContentLength || content.Length > MaxContentLength)
        {
            throw new ArgumentException(
                $"Treść recenzji musi mieć długość od {MinContentLength} do {MaxContentLength} znaków.",
                nameof(content));
        }
    }
}
