using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Exceptions;
using Xunit;

namespace UnitTests.ApplicationCore.Entities;

public class OrderTests
{
    private static Address ValidAddress() => new("1 Main St", "Redmond", "WA", "USA", "98052");

    [Fact]
    public void ConstructorThrowsGivenNoItems()
    {
        Assert.Throws<OrderDomainException>(() => new Order("buyer1", ValidAddress(), new List<OrderItem>()));
    }

    [Fact]
    public void TotalSumsUnitPriceTimesUnitsAcrossAllItems()
    {
        var items = new List<OrderItem>
        {
            new(new CatalogItemOrdered(1, "Mug", "mug.png"), 10m, 2),
            new(new CatalogItemOrdered(2, "T-Shirt", "shirt.png"), 15m, 1),
        };

        var order = new Order("buyer1", ValidAddress(), items);

        Assert.Equal(35m, order.Total());
    }

    [Fact]
    public void AddressValueObjectEqualityIsBasedOnValues()
    {
        var address1 = new Address("1 Main St", "Redmond", "WA", "USA", "98052");
        var address2 = new Address("1 Main St", "Redmond", "WA", "USA", "98052");

        Assert.Equal(address1, address2);
    }
}
