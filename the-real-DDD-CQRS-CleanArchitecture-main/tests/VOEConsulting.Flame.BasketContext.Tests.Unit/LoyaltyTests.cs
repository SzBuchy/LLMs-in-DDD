using System;
using System.Linq;
using FluentAssertions;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events;
using VOEConsulting.Flame.BasketContext.Tests.Unit.Extensions;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;
using Xunit;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class LoyaltyTests
    {
        private readonly Id<Customer> _customerId = Id<Customer>.New();

        [Fact]
        public void Create_ShouldInitializeCorrectlyAndRaiseEvent()
        {
            // Act
            var account = LoyaltyAccount.Create(_customerId);
            var expectedEvent = new LoyaltyAccountCreatedEvent(account.Id, _customerId);

            // Assert
            account.CustomerId.Should().Be(_customerId);
            account.PointsEntries.Should().BeEmpty();
            account.DomainEvents.Should().HaveCount(1);
            account.DomainEvents.Single().Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void AddPoints_ShouldAddEntryAndRaiseEvent()
        {
            // Arrange
            var account = LoyaltyAccount.Create(_customerId);
            var earnedAt = DateTimeOffset.UtcNow;

            // Act
            account.AddPoints(100, earnedAt);
            var expectedEvent = new PointsAddedEvent(account.Id, _customerId, 100, earnedAt);

            // Assert
            account.PointsEntries.Should().HaveCount(1);
            var entry = account.PointsEntries.Single();
            entry.Amount.Should().Be(100);
            entry.UsedAmount.Should().Be(0);
            entry.EarnedAtUtc.Should().Be(earnedAt);
            entry.ExpiresAtUtc.Should().Be(earnedAt.AddYears(1));
            account.GetAvailablePointsBalance(earnedAt).Should().Be(100);

            account.DomainEvents.Should().HaveCount(2); // Created + Added
            account.DomainEvents.Last().Should().BeEquivalentEventTo(expectedEvent);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void AddPoints_WhenAmountIsInvalid_ShouldThrowValidationException(int invalidAmount)
        {
            // Arrange
            var account = LoyaltyAccount.Create(_customerId);
            var earnedAt = DateTimeOffset.UtcNow;

            // Act & Assert
            var action = () => account.AddPoints(invalidAmount, earnedAt);
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public void GetAvailablePointsBalance_ShouldOnlyIncludeActiveNonExpiredPoints()
        {
            // Arrange
            var account = LoyaltyAccount.Create(_customerId);
            var now = DateTimeOffset.UtcNow;

            // Earned 14 months ago - Expired
            account.AddPoints(100, now.AddMonths(-14));

            // Earned 6 months ago - Active
            account.AddPoints(200, now.AddMonths(-6));

            // Earned just now - Active
            account.AddPoints(50, now);

            // Act
            var balance = account.GetAvailablePointsBalance(now);

            // Assert
            balance.Should().Be(250); // 200 + 50
        }

        [Fact]
        public void RedeemPoints_ShouldConsumePointsUsingFifo()
        {
            // Arrange
            var account = LoyaltyAccount.Create(_customerId);
            var now = DateTimeOffset.UtcNow;

            // Add three entries
            var firstEarnedAt = now.AddMonths(-6);
            var secondEarnedAt = now.AddMonths(-3);
            var thirdEarnedAt = now;

            account.AddPoints(100, firstEarnedAt); // Oldest
            account.AddPoints(150, secondEarnedAt);
            account.AddPoints(80, thirdEarnedAt); // Newest

            // Act - Redeem 120 points
            account.RedeemPoints(120, now);

            // Assert
            // 120 points should consume:
            // - 100 points from the first entry (completely used)
            // - 20 points from the second entry (leaving 130)
            // - 0 points from the third entry (leaving 80)
            account.GetAvailablePointsBalance(now).Should().Be(210); // (100 + 150 + 80) - 120 = 210

            var entries = account.PointsEntries.OrderBy(e => e.EarnedAtUtc).ToList();
            entries[0].UsedAmount.Should().Be(100);
            entries[0].GetAvailablePoints(now).Should().Be(0);

            entries[1].UsedAmount.Should().Be(20);
            entries[1].GetAvailablePoints(now).Should().Be(130);

            entries[2].UsedAmount.Should().Be(0);
            entries[2].GetAvailablePoints(now).Should().Be(80);
        }

        [Fact]
        public void RedeemPoints_WhenExceedingLimit_ShouldThrowValidationException()
        {
            // Arrange
            var account = LoyaltyAccount.Create(_customerId);
            var now = DateTimeOffset.UtcNow;
            account.AddPoints(600, now);

            // Act & Assert
            var action = () => account.RedeemPoints(550, now); // Exceeds MaxPointsPerRedemption = 500
            action.Should().Throw<ValidationException>().WithMessage("*500*");
        }

        [Fact]
        public void RedeemPoints_WhenInsufficientPoints_ShouldThrowValidationException()
        {
            // Arrange
            var account = LoyaltyAccount.Create(_customerId);
            var now = DateTimeOffset.UtcNow;
            account.AddPoints(100, now);

            // Act & Assert
            var action = () => account.RedeemPoints(150, now);
            action.Should().Throw<ValidationException>().WithMessage("*Only 100 points are available*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void RedeemPoints_WhenAmountIsInvalid_ShouldThrowValidationException(int invalidAmount)
        {
            // Arrange
            var account = LoyaltyAccount.Create(_customerId);
            var now = DateTimeOffset.UtcNow;

            // Act & Assert
            var action = () => account.RedeemPoints(invalidAmount, now);
            action.Should().Throw<ValidationException>();
        }
    }
}
