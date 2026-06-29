using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Profiles;
using VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Repositories;
using VOEConsulting.Infrastructure.Persistence;
using VOEConsulting.Flame.Common.Domain;
using Xunit;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class ReviewRepositoryTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<BasketAppDbContext> _dbContextOptions;

        public ReviewRepositoryTests()
        {
            // Initialize AutoMapper with the ReviewProfile
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ReviewProfile>();
            });
            _mapper = config.CreateMapper();

            // Initialize DbContextOptions with InMemory database provider
            _dbContextOptions = new DbContextOptionsBuilder<BasketAppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task AddAsync_ShouldAddReviewEntityToDatabase()
        {
            // Arrange
            using var dbContext = new BasketAppDbContext(_dbContextOptions);
            var repository = new ReviewRepository(dbContext, _mapper);

            var customerId = Id<Customer>.New();
            var productId = Id<Product>.New();
            var review = Review.Create(customerId, productId, 5, "This is a valid review content with enough characters.");

            // Act
            await repository.AddAsync(review);
            await dbContext.SaveChangesAsync();

            // Assert
            var savedEntity = await dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == review.Id.Value);
            savedEntity.Should().NotBeNull();
            savedEntity!.CustomerId.Should().Be(customerId.Value);
            savedEntity.ProductId.Should().Be(productId.Value);
            savedEntity.Rating.Should().Be(5);
            savedEntity.Content.Should().Be("This is a valid review content with enough characters.");
            savedEntity.Status.Should().Be((int)ReviewStatus.PendingModeration);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReconstituteReviewCorrectly()
        {
            // Arrange
            using var dbContext = new BasketAppDbContext(_dbContextOptions);
            var repository = new ReviewRepository(dbContext, _mapper);

            var reviewId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var entity = new ReviewEntity
            {
                Id = reviewId,
                CustomerId = customerId,
                ProductId = productId,
                Rating = 4,
                Content = "This is another valid review content.",
                Status = (int)ReviewStatus.Published
            };

            await dbContext.Reviews.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(Id<Review>.FromGuid(reviewId));

            // Assert
            result.Should().NotBeNull();
            result!.Id.Value.Should().Be(reviewId);
            result.CustomerId.Value.Should().Be(customerId);
            result.ProductId.Value.Should().Be(productId);
            result.Rating.Should().Be(4);
            result.Content.Should().Be("This is another valid review content.");
            result.Status.Should().Be(ReviewStatus.Published);
            result.DomainEvents.Should().BeEmpty(); // Verify no event was raised during reconstitution
        }

        [Fact]
        public async Task GetByProductIdAsync_ShouldReturnReviewsForProduct()
        {
            // Arrange
            using var dbContext = new BasketAppDbContext(_dbContextOptions);
            var repository = new ReviewRepository(dbContext, _mapper);

            var productId = Guid.NewGuid();
            var otherProductId = Guid.NewGuid();

            var entity1 = new ReviewEntity
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ProductId = productId,
                Rating = 5,
                Content = "Love this product, highly recommended!",
                Status = (int)ReviewStatus.Published
            };

            var entity2 = new ReviewEntity
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ProductId = productId,
                Rating = 2,
                Content = "Not good, broke after a few days.",
                Status = (int)ReviewStatus.Rejected
            };

            var entityOther = new ReviewEntity
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ProductId = otherProductId,
                Rating = 4,
                Content = "Good value for money.",
                Status = (int)ReviewStatus.Published
            };

            await dbContext.Reviews.AddRangeAsync(entity1, entity2, entityOther);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetByProductIdAsync(Id<Product>.FromGuid(productId));

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.ProductId.Value).Should().AllBeEquivalentTo(productId);
            result.Select(r => r.Content).Should().Contain(new[] { "Love this product, highly recommended!", "Not good, broke after a few days." });
        }

        [Fact]
        public async Task HasPublishedReviewForProductAsync_ShouldReturnTrue_WhenPublishedReviewExists()
        {
            // Arrange
            using var dbContext = new BasketAppDbContext(_dbContextOptions);
            var repository = new ReviewRepository(dbContext, _mapper);

            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var entity = new ReviewEntity
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = productId,
                Rating = 5,
                Content = "Excellent product quality.",
                Status = (int)ReviewStatus.Published
            };

            await dbContext.Reviews.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.HasPublishedReviewForProductAsync(
                Id<Customer>.FromGuid(customerId),
                Id<Product>.FromGuid(productId)
            );

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasPublishedReviewForProductAsync_ShouldReturnFalse_WhenNoReviewExistsOrNotPublished()
        {
            // Arrange
            using var dbContext = new BasketAppDbContext(_dbContextOptions);
            var repository = new ReviewRepository(dbContext, _mapper);

            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var entity = new ReviewEntity
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = productId,
                Rating = 5,
                Content = "Excellent product quality.",
                Status = (int)ReviewStatus.PendingModeration // not published
            };

            await dbContext.Reviews.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.HasPublishedReviewForProductAsync(
                Id<Customer>.FromGuid(customerId),
                Id<Product>.FromGuid(productId)
            );

            // Assert
            result.Should().BeFalse();
        }
    }
}
