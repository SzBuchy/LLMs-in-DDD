using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.ProductReviewServiceTests;

public class ProductReviewServiceTests
{
    private readonly IRepository<CatalogItem> _mockItemRepository = Substitute.For<IRepository<CatalogItem>>();
    private readonly IProductReviewRepository _mockReviewRepository = Substitute.For<IProductReviewRepository>();
    private readonly string _validCustomerId = "customer-123";
    private readonly int _validCatalogItemId = 42;
    private readonly int _validRating = 5;
    private readonly string _validTextContent = "This is a valid product review with enough characters.";

    private ProductReviewService CreateService()
    {
        _mockReviewRepository.AddAsync(Arg.Any<ProductReview>(), Arg.Any<CancellationToken>())
            .Returns(x => (ProductReview)x[0]);
        return new ProductReviewService(_mockItemRepository, _mockReviewRepository);
    }

    [Fact]
    public async Task AddsReviewSuccessfully()
    {
        var product = new CatalogItem(1, 1, "Test product", "Test product description", 10.0m, "picture.png");
        _mockItemRepository.GetByIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>()).Returns(product);
        
        _mockReviewRepository.GetByProductIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>())
            .Returns(new List<ProductReview>());

        var service = CreateService();

        var result = await service.AddProductReviewAsync(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);

        Assert.NotNull(result);
        Assert.Equal(_validCustomerId, result.CustomerId);
        Assert.Equal(_validCatalogItemId, result.CatalogItemId);
        Assert.Equal(_validRating, result.Rating);
        Assert.Equal(_validTextContent, result.TextContent);
        Assert.Equal(ReviewStatus.PendingModeration, result.Status);

        await _mockReviewRepository.Received(1).AddAsync(Arg.Is<ProductReview>(r => 
            r.CustomerId == _validCustomerId &&
            r.CatalogItemId == _validCatalogItemId &&
            r.Rating == _validRating &&
            r.TextContent == _validTextContent), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsGivenNonExistingProduct()
    {
        _mockItemRepository.GetByIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>()).Returns((CatalogItem?)null);

        var service = CreateService();

        await Assert.ThrowsAsync<CatalogItemNotFoundException>(() => 
            service.AddProductReviewAsync(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent));
    }

    [Fact]
    public async Task ThrowsGivenUserAlreadyHasPublishedReview()
    {
        var product = new CatalogItem(1, 1, "Test product", "Test product description", 10.0m, "picture.png");
        _mockItemRepository.GetByIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>()).Returns(product);

        var existingPublishedReview = new ProductReview(_validCustomerId, _validCatalogItemId, 4, "Another valid review text here.");
        existingPublishedReview.Approve(); // Mark as Published

        _mockReviewRepository.GetByProductIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>())
            .Returns(new List<ProductReview> { existingPublishedReview });

        var service = CreateService();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.AddProductReviewAsync(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent));

        Assert.Contains("already has a published review", exception.Message);
    }

    [Fact]
    public async Task SucceedsWhenUserHasOnlyPendingOrRejectedOrWithdrawnReviews()
    {
        var product = new CatalogItem(1, 1, "Test product", "Test product description", 10.0m, "picture.png");
        _mockItemRepository.GetByIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>()).Returns(product);

        var pendingReview = new ProductReview(_validCustomerId, _validCatalogItemId, 4, "Another valid review text here.");
        var rejectedReview = new ProductReview(_validCustomerId, _validCatalogItemId, 3, "Another valid review text here.");
        rejectedReview.Reject();
        var withdrawnReview = new ProductReview(_validCustomerId, _validCatalogItemId, 2, "Another valid review text here.");
        withdrawnReview.Withdraw();

        _mockReviewRepository.GetByProductIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>())
            .Returns(new List<ProductReview> { pendingReview, rejectedReview, withdrawnReview });

        var service = CreateService();

        var result = await service.AddProductReviewAsync(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);

        Assert.NotNull(result);
        await _mockReviewRepository.Received(1).AddAsync(Arg.Any<ProductReview>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task ThrowsGivenRatingOutOfRange(int invalidRating)
    {
        var product = new CatalogItem(1, 1, "Test product", "Test product description", 10.0m, "picture.png");
        _mockItemRepository.GetByIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>()).Returns(product);
        _mockReviewRepository.GetByProductIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>())
            .Returns(new List<ProductReview>());

        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
            service.AddProductReviewAsync(_validCustomerId, _validCatalogItemId, invalidRating, _validTextContent));
    }

    [Theory]
    [InlineData("Short")]
    [InlineData("123456789")]
    public async Task ThrowsGivenTextContentTooShort(string invalidTextContent)
    {
        var product = new CatalogItem(1, 1, "Test product", "Test product description", 10.0m, "picture.png");
        _mockItemRepository.GetByIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>()).Returns(product);
        _mockReviewRepository.GetByProductIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>())
            .Returns(new List<ProductReview>());

        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.AddProductReviewAsync(_validCustomerId, _validCatalogItemId, _validRating, invalidTextContent));
    }

    [Fact]
    public async Task GetPublishedReviewByCustomerAndProductReturnsPublishedReview()
    {
        var publishedReview = new ProductReview(_validCustomerId, _validCatalogItemId, 4, "Another valid review text.");
        publishedReview.Approve(); // Publish
        _mockReviewRepository.GetByProductIdAsync(_validCatalogItemId, Arg.Any<CancellationToken>())
            .Returns(new List<ProductReview> { publishedReview });

        var service = CreateService();
        var result = await service.GetPublishedReviewByCustomerAndProductAsync(_validCustomerId, _validCatalogItemId);

        Assert.NotNull(result);
        Assert.Equal(publishedReview.Id, result.Id);
        Assert.Equal(ReviewStatus.Published, result.Status);
    }

    [Fact]
    public async Task GetReviewByIdReturnsCorrectReview()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, _validRating, _validTextContent);
        int testReviewId = 99;
        _mockReviewRepository.GetByIdAsync(testReviewId, Arg.Any<CancellationToken>()).Returns(review);

        var service = CreateService();
        var result = await service.GetReviewByIdAsync(testReviewId);

        Assert.NotNull(result);
        Assert.Equal(review.CustomerId, result.CustomerId);
    }

    [Fact]
    public async Task EditProductReviewSuccessfullyEditsAndUpdates()
    {
        var review = new ProductReview(_validCustomerId, _validCatalogItemId, 5, _validTextContent);
        review.Approve(); // Must be published to edit
        int testReviewId = 99;

        _mockReviewRepository.GetByIdAsync(testReviewId, Arg.Any<CancellationToken>()).Returns(review);

        var service = CreateService();
        var result = await service.EditProductReviewAsync(_validCustomerId, testReviewId, 3, "New edited review text here.");

        Assert.NotNull(result);
        Assert.Equal(3, result.Rating);
        Assert.Equal("New edited review text here.", result.TextContent);
        Assert.Equal(ReviewStatus.PendingModeration, result.Status);

        await _mockReviewRepository.Received(1).UpdateAsync(review, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditProductReviewThrowsWhenNotFound()
    {
        int testReviewId = 99;
        _mockReviewRepository.GetByIdAsync(testReviewId, Arg.Any<CancellationToken>()).Returns((ProductReview?)null);

        var service = CreateService();

        await Assert.ThrowsAsync<ProductReviewNotFoundException>(() => 
            service.EditProductReviewAsync(_validCustomerId, testReviewId, 3, "New edited review text here."));
    }

    [Fact]
    public async Task EditProductReviewThrowsWhenUserDoesNotOwnReview()
    {
        var review = new ProductReview("other-customer", _validCatalogItemId, 5, _validTextContent);
        review.Approve();
        int testReviewId = 99;
        _mockReviewRepository.GetByIdAsync(testReviewId, Arg.Any<CancellationToken>()).Returns(review);

        var service = CreateService();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.EditProductReviewAsync(_validCustomerId, testReviewId, 3, "New edited review text here."));
        Assert.Contains("only edit your own reviews", exception.Message);
    }
}
