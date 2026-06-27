using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.LoyaltyAccountTests;

public class LoyaltyAccountExpirePoints
{
    [Fact]
    public void ExcludesPointsOneYearAfterTheyWereAwarded()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var awardedAt = new DateTimeOffset(2026, 6, 28, 10, 0, 0, TimeSpan.Zero);
        account.AwardPointsForOrder(1, 100m, awardedAt);

        Assert.Equal(100, account.AvailablePoints(awardedAt.AddYears(1).AddTicks(-1)));
        Assert.Equal(0, account.AvailablePoints(awardedAt.AddYears(1)));
    }

    [Fact]
    public void ExpirePointsClearsRemainingExpiredPoints()
    {
        var account = new LoyaltyAccount("customer@test.com");
        var awardedAt = new DateTimeOffset(2026, 6, 28, 10, 0, 0, TimeSpan.Zero);
        account.AwardPointsForOrder(1, 100m, awardedAt);

        account.ExpirePoints(awardedAt.AddYears(1));

        Assert.Equal(0, account.PointGrants.Single().PointsRemaining);
    }
}
