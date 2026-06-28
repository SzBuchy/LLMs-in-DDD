using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.Infrastructure.Data;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories.ReviewRepositoryTests;

public class ReviewRepositoryTests
{
    private const int CatalogItemId = 42;
    private readonly CatalogContext _catalogContext;
    private readonly ReviewRepository _reviewRepository;

    public ReviewRepositoryTests()
    {
        var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(databaseName: $"TestCatalog-{Guid.NewGuid()}")
            .Options;

        _catalogContext = new CatalogContext(dbOptions);
        _reviewRepository = new ReviewRepository(_catalogContext);
    }

    [Fact]
    public async Task AddsReviewAndGetsItByReviewId()
    {
        var review = new Review("buyer-1", CatalogItemId, 5, "Produkt spełnił wszystkie moje oczekiwania.");

        var savedReview = await _reviewRepository.AddAsync(review, TestContext.Current.CancellationToken);
        var reviewFromRepo = await _reviewRepository.GetByIdAsync(savedReview.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(reviewFromRepo);
        Assert.Equal(savedReview.Id, reviewFromRepo.Id);
        Assert.Equal("buyer-1", reviewFromRepo.BuyerId);
        Assert.Equal(CatalogItemId, reviewFromRepo.CatalogItemId);
        Assert.Equal(5, reviewFromRepo.Rating);
    }

    [Fact]
    public async Task GetsReviewsByCatalogItemId()
    {
        var matchingReview = new Review("buyer-1", CatalogItemId, 5, "Produkt spełnił wszystkie moje oczekiwania.");
        var secondMatchingReview = new Review("buyer-2", CatalogItemId, 4, "Dobry produkt, bardzo przydatny na co dzień.");
        var otherProductReview = new Review("buyer-3", 100, 3, "Ten produkt jest całkiem poprawny.");

        await _reviewRepository.AddAsync(matchingReview, TestContext.Current.CancellationToken);
        await _reviewRepository.AddAsync(secondMatchingReview, TestContext.Current.CancellationToken);
        await _reviewRepository.AddAsync(otherProductReview, TestContext.Current.CancellationToken);

        var reviews = await _reviewRepository.ListByCatalogItemIdAsync(CatalogItemId, TestContext.Current.CancellationToken);

        Assert.Equal(2, reviews.Count);
        Assert.All(reviews, review => Assert.Equal(CatalogItemId, review.CatalogItemId));
    }

    [Fact]
    public async Task UpdatesReview()
    {
        var review = new Review("buyer-1", CatalogItemId, 5, "Produkt spełnił wszystkie moje oczekiwania.");
        review.Publish();
        var savedReview = await _reviewRepository.AddAsync(review, TestContext.Current.CancellationToken);

        savedReview.Edit(3, "Produkt po edycji wymaga ponownej moderacji.");
        await _reviewRepository.UpdateAsync(savedReview, TestContext.Current.CancellationToken);

        var reviewFromRepo = await _reviewRepository.GetByIdAsync(savedReview.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(reviewFromRepo);
        Assert.Equal(3, reviewFromRepo.Rating);
        Assert.Equal("Produkt po edycji wymaga ponownej moderacji.", reviewFromRepo.Content);
        Assert.Equal(ReviewStatus.PendingModeration, reviewFromRepo.Status);
    }
}
