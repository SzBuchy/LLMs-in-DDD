using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.Infrastructure.Data;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories.ProductReviewRepositoryTests;

public class ProductReviewRepositoryTests
{
    private readonly CatalogContext _catalogContext;
    private readonly ProductReviewRepository _reviewRepository;
    private readonly ITestOutputHelper _output;

    private readonly string _validCustomerId = "customer-123";
    private readonly int _validCatalogItemId = 42;
    private readonly int _validRating = 5;
    private readonly string _validTextContent = "This is a valid product review with enough characters.";

    public ProductReviewRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
        var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _catalogContext = new CatalogContext(dbOptions);
        _reviewRepository = new ProductReviewRepository(_catalogContext);
    }

    [Fact]
    public async Task AddsAndRetrievesReviewById()
    {
        // Arrange
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);

        // Act
        var addedReview = await _reviewRepository.AddAsync(review, TestContext.Current.CancellationToken);
        _output.WriteLine($"Added ProductReview with ID: {addedReview.Id}");

        var retrievedReview = await _reviewRepository.GetByIdAsync(addedReview.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(retrievedReview);
        Assert.Equal(_validCustomerId, retrievedReview.CustomerId);
        Assert.Equal(_validCatalogItemId, retrievedReview.CatalogItemId);
        Assert.Equal(_validRating, retrievedReview.Rating);
        Assert.Equal(_validTextContent, retrievedReview.TextContent);
        Assert.Equal(ReviewStatus.PendingModeration, retrievedReview.Status);
    }

    [Fact]
    public async Task RetrievesReviewsByProductId()
    {
        // Arrange
        int productId1 = 101;
        int productId2 = 102;
        var review1 = new ProductReview(_validCustomerId, productId1, _validRating, _validTextContent);
        var review2 = new ProductReview(_validCustomerId, productId1, 4, "Another review for product 101.");
        var review3 = new ProductReview(_validCustomerId, productId2, _validRating, "Review for product 102.");

        await _reviewRepository.AddAsync(review1, TestContext.Current.CancellationToken);
        await _reviewRepository.AddAsync(review2, TestContext.Current.CancellationToken);
        await _reviewRepository.AddAsync(review3, TestContext.Current.CancellationToken);

        // Act
        var reviewsForProduct1 = (await _reviewRepository.GetByProductIdAsync(productId1, TestContext.Current.CancellationToken)).ToList();

        // Assert
        Assert.Equal(2, reviewsForProduct1.Count);
        Assert.Contains(reviewsForProduct1, r => r.TextContent == _validTextContent);
        Assert.Contains(reviewsForProduct1, r => r.TextContent == "Another review for product 101.");
        Assert.DoesNotContain(reviewsForProduct1, r => r.TextContent == "Review for product 102.");
    }

    [Fact]
    public async Task UpdatesReviewStatus()
    {
        // Arrange
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var addedReview = await _reviewRepository.AddAsync(review, TestContext.Current.CancellationToken);

        // Act
        addedReview.Approve();
        await _reviewRepository.UpdateAsync(addedReview, TestContext.Current.CancellationToken);

        var retrievedReview = await _reviewRepository.GetByIdAsync(addedReview.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(retrievedReview);
        Assert.Equal(ReviewStatus.Published, retrievedReview.Status);
    }

    [Fact]
    public async Task DeletesReview()
    {
        // Arrange
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        var addedReview = await _reviewRepository.AddAsync(review, TestContext.Current.CancellationToken);

        // Act
        await _reviewRepository.DeleteAsync(addedReview, TestContext.Current.CancellationToken);
        var retrievedReview = await _reviewRepository.GetByIdAsync(addedReview.Id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(retrievedReview);
    }
}
