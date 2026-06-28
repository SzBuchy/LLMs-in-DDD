using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities;

public class ReviewTests
{
    private readonly string _validBuyerId = "test-buyer-id";
    private readonly int _validCatalogItemId = 42;
    private readonly int _validRating = 4;
    private readonly string _validContent = "To jest bardzo dobry produkt, gorąco polecam!"; // 45 chars

    [Fact]
    public void CreatesReviewSuccessfullyWithValidDetails()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);

        Assert.Equal(_validBuyerId, review.BuyerId);
        Assert.Equal(_validCatalogItemId, review.CatalogItemId);
        Assert.Equal(_validRating, review.Rating);
        Assert.Equal(_validContent, review.Content);
        Assert.Equal(ReviewStatus.PendingModeration, review.Status);
    }

    [Fact]
    public void ConstructorThrowsArgumentExceptionGivenEmptyBuyerId()
    {
        Assert.Throws<ArgumentException>(() => new Review(string.Empty, _validCatalogItemId, _validRating, _validContent));
    }

    [Fact]
    public void ConstructorThrowsArgumentNullExceptionGivenNullBuyerId()
    {
        Assert.Throws<ArgumentNullException>(() => new Review(null!, _validCatalogItemId, _validRating, _validContent));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ConstructorThrowsArgumentExceptionGivenInvalidCatalogItemId(int invalidCatalogItemId)
    {
        Assert.Throws<ArgumentException>(() => new Review(_validBuyerId, invalidCatalogItemId, _validRating, _validContent));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void ConstructorThrowsArgumentOutOfRangeExceptionGivenInvalidRating(int invalidRating)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Review(_validBuyerId, _validCatalogItemId, invalidRating, _validContent));
    }

    [Theory]
    [InlineData("Krótka")] // < 10 chars
    [InlineData("")] // empty
    public void ConstructorThrowsArgumentExceptionGivenInvalidContent(string invalidContent)
    {
        Assert.Throws<ArgumentException>(() => new Review(_validBuyerId, _validCatalogItemId, _validRating, invalidContent));
    }

    [Fact]
    public void ConstructorThrowsArgumentNullExceptionGivenNullContent()
    {
        Assert.Throws<ArgumentNullException>(() => new Review(_validBuyerId, _validCatalogItemId, _validRating, null!));
    }

    [Fact]
    public void ConstructorThrowsArgumentExceptionGivenTooLongContent()
    {
        var tooLongContent = new string('A', 501);
        Assert.Throws<ArgumentException>(() => new Review(_validBuyerId, _validCatalogItemId, _validRating, tooLongContent));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(500)]
    public void CreatesReviewGivenBoundaryContentLength(int contentLength)
    {
        var content = new string('A', contentLength);

        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, content);

        Assert.Equal(content, review.Content);
    }

    [Fact]
    public void PublishChangesStatusToPublishedWhenPending()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);

        review.Publish();

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void PublishThrowsInvalidOperationExceptionWhenAlreadyPublished()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);
        review.Publish();

        Assert.Throws<InvalidOperationException>(() => review.Publish());
    }

    [Fact]
    public void RejectChangesStatusToRejectedWhenPending()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);

        review.Reject();

        Assert.Equal(ReviewStatus.Rejected, review.Status);
    }

    [Fact]
    public void RejectThrowsInvalidOperationExceptionWhenAlreadyRejected()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);
        review.Reject();

        Assert.Throws<InvalidOperationException>(() => review.Reject());
    }

    [Fact]
    public void PublishThrowsInvalidOperationExceptionWhenRejected()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);
        review.Reject();

        Assert.Throws<InvalidOperationException>(() => review.Publish());
    }

    [Fact]
    public void RejectThrowsInvalidOperationExceptionWhenPublished()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);
        review.Publish();

        Assert.Throws<InvalidOperationException>(() => review.Reject());
    }

    [Fact]
    public void EditUpdatesPublishedReviewAndSendsItBackToModeration()
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);
        review.Publish();
        const int newRating = 3;
        const string newContent = "Produkt po czasie nadal jest całkiem dobry.";

        review.Edit(newRating, newContent);

        Assert.Equal(newRating, review.Rating);
        Assert.Equal(newContent, review.Content);
        Assert.Equal(ReviewStatus.PendingModeration, review.Status);
    }

    [Theory]
    [InlineData(ReviewStatus.PendingModeration)]
    [InlineData(ReviewStatus.Rejected)]
    public void EditThrowsReviewNotEditableExceptionWhenReviewIsNotPublished(ReviewStatus reviewStatus)
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);
        if (reviewStatus == ReviewStatus.Rejected)
        {
            review.Reject();
        }

        Assert.Throws<ReviewNotEditableException>(() => review.Edit(3, "Produkt wymaga ponownej oceny klienta."));
    }

    [Theory]
    [InlineData(0, "Produkt wymaga ponownej oceny klienta.")]
    [InlineData(6, "Produkt wymaga ponownej oceny klienta.")]
    [InlineData(3, "Krótka")]
    public void EditThrowsArgumentExceptionGivenInvalidDetails(int rating, string content)
    {
        var review = new Review(_validBuyerId, _validCatalogItemId, _validRating, _validContent);
        review.Publish();

        Assert.ThrowsAny<ArgumentException>(() => review.Edit(rating, content));
    }
}
