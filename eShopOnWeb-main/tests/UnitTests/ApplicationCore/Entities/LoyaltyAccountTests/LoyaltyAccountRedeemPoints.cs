using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.LoyaltyAccountTests;

public class LoyaltyAccountRedeemPoints
{
    [Fact]
    public void RedeemsPointsAndCalculatesDiscount()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var awardedAt = new DateTimeOffset(2026, 6, 28, 10, 0, 0, TimeSpan.Zero);
        account.AwardPointsForOrder(1, 100m, awardedAt);

        var redemption = account.RedeemPoints(40, awardedAt.AddDays(1));

        Assert.Equal(40, redemption.PointsRedeemed);
        Assert.Equal(0.40m, redemption.DiscountAmount);
        Assert.Equal(60, account.AvailablePoints(awardedAt.AddDays(1)));
    }

    [Fact]
    public void DoesNotAllowRedeemingMoreThanTheSingleUseLimit()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var awardedAt = new DateTimeOffset(2026, 6, 28, 10, 0, 0, TimeSpan.Zero);
        account.AwardPointsForOrder(1, 1000m, awardedAt);

        Assert.Throws<LoyaltyAccountOperationException>(() =>
            account.RedeemPoints(LoyaltyAccount.MaxPointsPerRedemption + 1, awardedAt.AddDays(1)));
    }

    [Fact]
    public void DoesNotAllowRedeemingMoreActivePointsThanAvailable()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var awardedAt = new DateTimeOffset(2026, 6, 28, 10, 0, 0, TimeSpan.Zero);
        account.AwardPointsForOrder(1, 25m, awardedAt);

        Assert.Throws<LoyaltyAccountOperationException>(() =>
            account.RedeemPoints(26, awardedAt.AddDays(1)));
    }

    [Fact]
    public void RedeemsPointsFromTheGrantThatExpiresFirst()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var olderAward = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var newerAward = new DateTimeOffset(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
        account.AwardPointsForOrder(1, 100m, olderAward);
        account.AwardPointsForOrder(2, 100m, newerAward);

        account.RedeemPoints(80, newerAward.AddDays(1));

        var olderGrant = account.PointGrants.Single(grant => grant.SourceOrderId == 1);
        var newerGrant = account.PointGrants.Single(grant => grant.SourceOrderId == 2);
        Assert.Equal(20, olderGrant.PointsRemaining);
        Assert.Equal(100, newerGrant.PointsRemaining);
    }
}
