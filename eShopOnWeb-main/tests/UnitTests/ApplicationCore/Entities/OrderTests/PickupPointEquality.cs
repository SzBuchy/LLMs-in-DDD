using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.OrderTests;

public class PickupPointEquality
{
    [Fact]
    public void IsEqualWhenAllAttributesHaveTheSameValues()
    {
        var firstPickupPoint = new PickupPoint("Parcel Locker 123", "Main St. 1", "Kent", "44240");
        var secondPickupPoint = new PickupPoint("Parcel Locker 123", "Main St. 1", "Kent", "44240");

        Assert.Equal(firstPickupPoint, secondPickupPoint);
    }

    [Fact]
    public void IsNotEqualWhenAnyAttributeHasDifferentValue()
    {
        var firstPickupPoint = new PickupPoint("Parcel Locker 123", "Main St. 1", "Kent", "44240");
        var secondPickupPoint = new PickupPoint("Parcel Locker 456", "Main St. 1", "Kent", "44240");

        Assert.NotEqual(firstPickupPoint, secondPickupPoint);
    }
}
