using System;
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
}
