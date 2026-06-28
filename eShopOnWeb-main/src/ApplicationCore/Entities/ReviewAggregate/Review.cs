using System;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

public class Review : BaseEntity, IAggregateRoot
{
    public const int MinContentLength = 10;
    public const int MaxContentLength = 500;
    public const int MinRating = 1;
    public const int MaxRating = 5;

#pragma warning disable CS8618 // Required by Entity Framework
    private Review() { }
#pragma warning restore CS8618

    public Review(string buyerId, int catalogItemId, int rating, string content)
    {
        Guard.Against.NullOrEmpty(buyerId, nameof(buyerId));
        Guard.Against.NegativeOrZero(catalogItemId, nameof(catalogItemId));
        Guard.Against.OutOfRange(rating, nameof(rating), MinRating, MaxRating);
        GuardAgainstInvalidContent(content);

        BuyerId = buyerId;
        CatalogItemId = catalogItemId;
        Rating = rating;
        Content = content;
        Status = ReviewStatus.PendingModeration;
    }

    public string BuyerId { get; private set; }
    public int CatalogItemId { get; private set; }
    public int Rating { get; private set; }
    public string Content { get; private set; }
    public ReviewStatus Status { get; private set; }

    public void Publish()
    {
        if (Status != ReviewStatus.PendingModeration)
        {
            throw new InvalidOperationException(
                $"Review can only be published from '{ReviewStatus.PendingModeration}' status, but current status is '{Status}'.");
        }

        Status = ReviewStatus.Published;
    }

    // Editing an already published review sends it back for moderation, reusing the
    // same validation rules that apply when a review is first submitted.
    public void Edit(int rating, string content)
    {
        if (Status != ReviewStatus.Published)
        {
            throw new InvalidOperationException(
                $"Review can only be edited from '{ReviewStatus.Published}' status, but current status is '{Status}'.");
        }

        Guard.Against.OutOfRange(rating, nameof(rating), MinRating, MaxRating);
        GuardAgainstInvalidContent(content);

        Rating = rating;
        Content = content;
        Status = ReviewStatus.PendingModeration;
    }

    public void Reject()
    {
        if (Status != ReviewStatus.PendingModeration)
        {
            throw new InvalidOperationException(
                $"Review can only be rejected from '{ReviewStatus.PendingModeration}' status, but current status is '{Status}'.");
        }

        Status = ReviewStatus.Rejected;
    }

    private static void GuardAgainstInvalidContent(string content)
    {
        Guard.Against.NullOrEmpty(content, nameof(content));
        if (content.Length < MinContentLength || content.Length > MaxContentLength)
        {
            throw new ArgumentException(
                $"Review content must be between {MinContentLength} and {MaxContentLength} characters long.",
                nameof(content));
        }
    }
}
