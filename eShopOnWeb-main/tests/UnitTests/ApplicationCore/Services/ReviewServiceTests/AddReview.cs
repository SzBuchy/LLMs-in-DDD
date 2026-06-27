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

public class AddReview
{
    private const string BuyerId = "demouser@microsoft.com";
    private const int CatalogItemId = 1;
    private const int Rating = 5;
    private const string Content = "Produkt jest bardzo dobry i spełnia oczekiwania.";

    private readonly IReadRepository<CatalogItem> _catalogItemRepository = Substitute.For<IReadRepository<CatalogItem>>();
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();

    [Fact]
    public async Task AddsPendingReviewGivenExistingCatalogItem()
    {
        var catalogItem = new CatalogItem(1, 1, "Description", "Name", 10m, "image.png");
        _catalogItemRepository.GetByIdAsync(CatalogItemId, Arg.Any<CancellationToken>()).Returns(catalogItem);
        _reviewRepository.AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Review>());

        var service = new ReviewService(_catalogItemRepository, _reviewRepository);

        var review = await service.AddReviewAsync(BuyerId, CatalogItemId, Rating, Content);

        Assert.Equal(BuyerId, review.BuyerId);
        Assert.Equal(CatalogItemId, review.CatalogItemId);
        Assert.Equal(Rating, review.Rating);
        Assert.Equal(Content, review.Content);
        Assert.Equal(ReviewStatus.PendingModeration, review.Status);
        await _reviewRepository.Received(1).AddAsync(review, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsCatalogItemNotFoundGivenMissingCatalogItem()
    {
        _catalogItemRepository.GetByIdAsync(CatalogItemId, Arg.Any<CancellationToken>())
            .Returns((CatalogItem?)null);

        var service = new ReviewService(_catalogItemRepository, _reviewRepository);

        await Assert.ThrowsAsync<CatalogItemNotFoundException>(() =>
            service.AddReviewAsync(BuyerId, CatalogItemId, Rating, Content));

        await _reviewRepository.DidNotReceive().AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
    }
}
