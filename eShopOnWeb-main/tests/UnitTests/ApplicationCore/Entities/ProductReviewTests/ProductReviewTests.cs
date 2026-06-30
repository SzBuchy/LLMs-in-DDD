using System;
using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.ProductReviewTests;

public class ProductReviewTests
{
    private readonly string _validCustomerId = "customer-123";
    private readonly int _validCatalogItemId = 42;
    private readonly int _validRating = 5;
    private readonly string _validTextContent = "This is a valid product review with enough characters.";

    [Fact]
    public void CreatesReviewWithDefaultPendingStatus()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);

        Assert.Equal(_validCustomerId, review.CustomerId);
        Assert.Equal(_validCatalogItemId, review.CatalogItemId);
        Assert.Equal(_validRating, review.Rating);
        Assert.Equal(_validTextContent, review.TextContent);
        Assert.Equal(ReviewStatus.PendingModeration, review.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ThrowsGivenInvalidCustomerId(string? invalidCustomerId)
    {
        Assert.ThrowsAny<ArgumentException>(() => 
            new ProductReview(invalidCustomerId!, _validCatalogItemId, _validRating, _validTextContent));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ThrowsGivenInvalidCatalogItemId(int invalidCatalogItemId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new ProductReview(_validCustomerId, invalidCatalogItemId, _validRating, _validTextContent));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void ThrowsGivenRatingOutOfRange(int invalidRating)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new ProductReview(_validCustomerId, _validCatalogItemId, invalidRating, _validTextContent));
    }

    [Theory]
    [InlineData("Short")]
    [InlineData("123456789")]
    public void ThrowsGivenTextContentTooShort(string invalidTextContent)
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, invalidTextContent));
        Assert.Contains("between 10 and 500 characters", exception.Message);
    }

    [Fact]
    public void ThrowsGivenTextContentTooLong()
    {
        var longText = new string('a', 501);
        var exception = Assert.Throws<ArgumentException>(() => 
            new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, longText));
        Assert.Contains("between 10 and 500 characters", exception.Message);
    }

    [Fact]
    public void ApproveTransitionsStatusToPublished()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        
        review.Approve();

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void RejectTransitionsStatusToRejected()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        
        review.Reject();

        Assert.Equal(ReviewStatus.Rejected, review.Status);
    }

    [Fact]
    public void SendToModerationTransitionsStatusToPendingModeration()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        
        review.Approve(); // Transition to Published
        Assert.Equal(ReviewStatus.Published, review.Status);

        review.SendToModeration(); // Transition back to Pending

        Assert.Equal(ReviewStatus.PendingModeration, review.Status);
    }

    [Fact]
    public void WithdrawTransitionsStatusToWithdrawn()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        
        review.Withdraw();

        Assert.Equal(ReviewStatus.Withdrawn, review.Status);
    }

    [Fact]
    public void PublishSucceedsWhenNoExistingReviews()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var policy = new ProductReviewPublicationPolicy();
        var existingReviews = new List<ProductReview>();

        review.Publish(policy, existingReviews);

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void PublishSucceedsWhenExistingReviewsAreNotPublished()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var otherReview1 = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent); // status is Pending
        var otherReview2 = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        otherReview2.Reject(); // status is Rejected

        var policy = new ProductReviewPublicationPolicy();
        var existingReviews = new List<ProductReview> { otherReview1, otherReview2 };

        review.Publish(policy, existingReviews);

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void PublishThrowsWhenAnotherReviewForSameProductAndCustomerIsPublished()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var publishedReview = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        publishedReview.Approve(); // status is Published

        var policy = new ProductReviewPublicationPolicy();
        var existingReviews = new List<ProductReview> { publishedReview };

        Assert.Throws<InvalidOperationException>(() => review.Publish(policy, existingReviews));
    }

    [Fact]
    public void PublishSucceedsWhenPreviousReviewForSameProductAndCustomerIsWithdrawn()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var withdrawnReview = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        withdrawnReview.Withdraw(); // status is Withdrawn

        var policy = new ProductReviewPublicationPolicy();
        var existingReviews = new List<ProductReview> { withdrawnReview };

        review.Publish(policy, existingReviews);

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void PublishSucceedsWhenOtherPublishedReviewIsForDifferentProduct()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var otherProductReview = new ProductReview(_validCustomerId, _validCatalogItemId + 1, _validRating, _validTextContent);
        otherProductReview.Approve(); // status is Published

        var policy = new ProductReviewPublicationPolicy();
        var existingReviews = new List<ProductReview> { otherProductReview };

        review.Publish(policy, existingReviews);

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void PublishSucceedsWhenOtherPublishedReviewIsByDifferentCustomer()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var otherCustomerReview = new ProductReview("other-customer", _validCatalogItemId, _validRating, _validTextContent);
        otherCustomerReview.Approve(); // status is Published

        var policy = new ProductReviewPublicationPolicy();
        var existingReviews = new List<ProductReview> { otherCustomerReview };

        review.Publish(policy, existingReviews);

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void EditUpdatesReviewAndResetsStatusToPendingModeration()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        review.Approve(); // Transition to Published
        Assert.Equal(ReviewStatus.Published, review.Status);

        string newTextContent = "This is a new valid text content for editing.";
        int newRating = 3;

        review.Edit(newRating, newTextContent);

        Assert.Equal(newRating, review.Rating);
        Assert.Equal(newTextContent, review.TextContent);
        Assert.Equal(ReviewStatus.PendingModeration, review.Status);
    }

    [Theory]
    [InlineData(ReviewStatus.PendingModeration)]
    [InlineData(ReviewStatus.Rejected)]
    [InlineData(ReviewStatus.Withdrawn)]
    public void EditThrowsWhenReviewIsNotPublished(ReviewStatus status)
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        
        // Setup initial status
        if (status == ReviewStatus.Rejected)
        {
            review.Reject();
        }
        else if (status == ReviewStatus.Withdrawn)
        {
            review.Withdraw();
        }
        // If status is PendingModeration, do nothing (default)

        Assert.Equal(status, review.Status);

        var exception = Assert.Throws<InvalidOperationException>(() => 
            review.Edit(4, "This is another valid text content."));
        Assert.Contains("Only published reviews can be edited", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void EditThrowsGivenRatingOutOfRange(int invalidRating)
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        review.Approve(); // Publish

        Assert.Throws<ArgumentOutOfRangeException>(() => 
            review.Edit(invalidRating, _validTextContent));
    }

    [Theory]
    [InlineData("Short")]
    [InlineData("123456789")]
    public void EditThrowsGivenTextContentTooShort(string invalidTextContent)
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        review.Approve(); // Publish

        var exception = Assert.Throws<ArgumentException>(() => 
            review.Edit(_validRating, invalidTextContent));
        Assert.Contains("between 10 and 500 characters", exception.Message);
    }

    [Fact]
    public void EditThrowsGivenTextContentTooLong()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        review.Approve(); // Publish

        var longText = new string('a', 501);
        var exception = Assert.Throws<ArgumentException>(() => 
            review.Edit(_validRating, longText));
        Assert.Contains("between 10 and 500 characters", exception.Message);
    }
}
