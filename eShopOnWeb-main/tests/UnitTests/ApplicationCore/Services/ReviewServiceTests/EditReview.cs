using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.ReviewServiceTests;

public class EditReview
{
    private const string BuyerId = "demouser@microsoft.com";
    private const string OtherBuyerId = "otheruser@microsoft.com";
    private const int CatalogItemId = 1;
    private const int ReviewId = 10;
    private const int Rating = 4;
    private const string Content = "Produkt po edycji nadal jest wart polecenia.";

    private readonly IReadRepository<CatalogItem> _catalogItemRepository = Substitute.For<IReadRepository<CatalogItem>>();
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();

    [Fact]
    public async Task UpdatesPublishedReviewAndSendsItBackToModeration()
    {
        var review = CreatePublishedReview(BuyerId);
        _reviewRepository.GetByIdAsync(ReviewId, Arg.Any<CancellationToken>()).Returns(review);
        var service = new ReviewService(_catalogItemRepository, _reviewRepository);

        var updatedReview = await service.EditReviewAsync(BuyerId, CatalogItemId, ReviewId, Rating, Content);

        Assert.Equal(Rating, updatedReview.Rating);
        Assert.Equal(Content, updatedReview.Content);
        Assert.Equal(ReviewStatus.PendingModeration, updatedReview.Status);
        await _reviewRepository.Received(1).UpdateAsync(review, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsReviewNotFoundGivenMissingReview()
    {
        _reviewRepository.GetByIdAsync(ReviewId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);
        var service = new ReviewService(_catalogItemRepository, _reviewRepository);

        await Assert.ThrowsAsync<ReviewNotFoundException>(() =>
            service.EditReviewAsync(BuyerId, CatalogItemId, ReviewId, Rating, Content));

        await _reviewRepository.DidNotReceive().UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsReviewNotFoundGivenReviewFromOtherCatalogItem()
    {
        var review = CreatePublishedReview(BuyerId, catalogItemId: 2);
        _reviewRepository.GetByIdAsync(ReviewId, Arg.Any<CancellationToken>()).Returns(review);
        var service = new ReviewService(_catalogItemRepository, _reviewRepository);

        await Assert.ThrowsAsync<ReviewNotFoundException>(() =>
            service.EditReviewAsync(BuyerId, CatalogItemId, ReviewId, Rating, Content));

        await _reviewRepository.DidNotReceive().UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsReviewAccessDeniedGivenOtherBuyerReview()
    {
        var review = CreatePublishedReview(OtherBuyerId);
        _reviewRepository.GetByIdAsync(ReviewId, Arg.Any<CancellationToken>()).Returns(review);
        var service = new ReviewService(_catalogItemRepository, _reviewRepository);

        await Assert.ThrowsAsync<ReviewAccessDeniedException>(() =>
            service.EditReviewAsync(BuyerId, CatalogItemId, ReviewId, Rating, Content));

        await _reviewRepository.DidNotReceive().UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
    }

    private static Review CreatePublishedReview(string buyerId, int catalogItemId = CatalogItemId)
    {
        var review = new Review(buyerId, catalogItemId, 5, "Produkt jest bardzo dobry i spełnia oczekiwania.");
        review.Publish();
        review.GetType().GetProperty(nameof(Review.Id))!.SetValue(review, ReviewId);
        return review;
    }
}
