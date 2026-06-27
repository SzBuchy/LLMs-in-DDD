using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class LoyaltyAccountTests
    {
        private static readonly Id<Customer> CustomerId = Id<Customer>.New();
        private static readonly DateTimeOffset AwardedAtUtc = new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);

        [Fact]
        public void AwardPoints_WhenOrderIsNew_ShouldIncreaseAvailablePoints()
        {
            var account = LoyaltyAccount.Create(CustomerId, maxPointsPerRedemption: 100);

            account.AwardPoints(Guid.NewGuid(), 50, AwardedAtUtc);

            account.AvailablePoints(AwardedAtUtc).Should().Be(50);
            account.PointBatches.Single().ExpiresAtUtc.Should().Be(AwardedAtUtc.AddYears(1));
        }

        [Fact]
        public void AwardPoints_WhenOrderWasAlreadyAwarded_ShouldBeIdempotent()
        {
            var account = LoyaltyAccount.Create(CustomerId, maxPointsPerRedemption: 100);
            var orderId = Guid.NewGuid();

            account.AwardPoints(orderId, 50, AwardedAtUtc);
            account.AwardPoints(orderId, 50, AwardedAtUtc);

            account.AvailablePoints(AwardedAtUtc).Should().Be(50);
            account.PointBatches.Should().ContainSingle();
        }

        [Fact]
        public void RedeemPoints_WhenEnoughPointsAndWithinLimit_ShouldCreateDiscount()
        {
            var account = LoyaltyAccount.Create(CustomerId, maxPointsPerRedemption: 100);
            account.AwardPoints(Guid.NewGuid(), 80, AwardedAtUtc);

            var redemption = account.RedeemPoints(Guid.NewGuid(), 30, AwardedAtUtc.AddDays(1), pointValue: 0.10m);

            redemption.Points.Should().Be(30);
            redemption.DiscountAmount.Should().Be(3.00m);
            account.AvailablePoints(AwardedAtUtc.AddDays(1)).Should().Be(50);
        }

        [Fact]
        public void RedeemPoints_WhenRequestedPointsExceedLimit_ShouldFail()
        {
            var account = LoyaltyAccount.Create(CustomerId, maxPointsPerRedemption: 20);
            account.AwardPoints(Guid.NewGuid(), 80, AwardedAtUtc);

            var action = () => account.RedeemPoints(Guid.NewGuid(), 30, AwardedAtUtc.AddDays(1));

            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void RedeemPoints_WhenPointsAreExpired_ShouldFail()
        {
            var account = LoyaltyAccount.Create(CustomerId, maxPointsPerRedemption: 100);
            account.AwardPoints(Guid.NewGuid(), 80, AwardedAtUtc);

            var action = () => account.RedeemPoints(Guid.NewGuid(), 30, AwardedAtUtc.AddYears(1));

            action.Should().ThrowExactly<ValidationException>();
            account.AvailablePoints(AwardedAtUtc.AddYears(1)).Should().Be(0);
        }

        [Fact]
        public void ExpirePoints_WhenBatchIsOlderThanOneYear_ShouldMarkPointsExpired()
        {
            var account = LoyaltyAccount.Create(CustomerId, maxPointsPerRedemption: 100);
            account.AwardPoints(Guid.NewGuid(), 80, AwardedAtUtc);

            var expiredPoints = account.ExpirePoints(AwardedAtUtc.AddYears(1));

            expiredPoints.Should().Be(80);
            account.AvailablePoints(AwardedAtUtc.AddYears(1)).Should().Be(0);
            account.ExpiredPoints(AwardedAtUtc.AddYears(1)).Should().Be(80);
        }
    }
}
