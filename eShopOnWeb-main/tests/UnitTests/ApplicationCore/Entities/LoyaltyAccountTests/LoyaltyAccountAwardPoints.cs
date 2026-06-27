using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.LoyaltyAccountTests;

public class LoyaltyAccountAwardPoints
{
    [Fact]
    public void AwardsOnePointPerFullOrderCurrencyUnit()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var awardedAt = new DateTimeOffset(2026, 6, 28, 10, 0, 0, TimeSpan.Zero);

        var awardedPoints = account.AwardPointsForOrder(1, 123.99m, awardedAt);

        Assert.Equal(123, awardedPoints);
        Assert.Equal(123, account.AvailablePoints(awardedAt));
        Assert.Equal(awardedAt.AddYears(1), account.PointGrants.Single().ExpiresAt);
    }

    [Fact]
    public void DoesNotAwardPointsTwiceForTheSameOrder()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var awardedAt = new DateTimeOffset(2026, 6, 28, 10, 0, 0, TimeSpan.Zero);

        account.AwardPointsForOrder(1, 100m, awardedAt);
        var awardedPoints = account.AwardPointsForOrder(1, 100m, awardedAt);

        Assert.Equal(0, awardedPoints);
        Assert.Equal(100, account.AvailablePoints(awardedAt));
        Assert.Single(account.PointGrants);
    }
}
