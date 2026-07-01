using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Exceptions;
using Xunit;

namespace UnitTests.ApplicationCore.Entities;

public class BasketTests
{
    [Fact]
    public void ConstructorSetsBuyerId()
    {
        var basket = new Basket("buyer1");

        Assert.Equal("buyer1", basket.BuyerId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ConstructorThrowsGivenInvalidBuyerId(string? buyerId)
    {
        Assert.Throws<BasketDomainException>(() => new Basket(buyerId!));
    }

    [Fact]
    public void AddItemAddsNewLineWhenCatalogItemNotYetInBasket()
    {
        var basket = new Basket("buyer1");

        basket.AddItem(catalogItemId: 1, unitPrice: 10m, quantity: 2);

        var item = Assert.Single(basket.Items);
        Assert.Equal(1, item.CatalogItemId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(10m, item.UnitPrice);
    }

    [Fact]
    public void AddItemIncreasesQuantityWhenCatalogItemAlreadyInBasket()
    {
        var basket = new Basket("buyer1");

        basket.AddItem(catalogItemId: 1, unitPrice: 10m, quantity: 2);
        basket.AddItem(catalogItemId: 1, unitPrice: 10m, quantity: 3);

        var item = Assert.Single(basket.Items);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void AddItemThrowsGivenNonPositiveQuantity()
    {
        var basket = new Basket("buyer1");

        Assert.Throws<BasketDomainException>(() => basket.AddItem(1, 10m, 0));
    }

    [Fact]
    public void SetQuantitiesRemovesLinesSetToZero()
    {
        var basket = new Basket("buyer1");
        basket.AddItem(1, 10m, 2);
        var itemId = basket.Items.Single().Id;

        basket.SetQuantities(new Dictionary<int, int> { [itemId] = 0 });

        Assert.Empty(basket.Items);
    }

    [Fact]
    public void SetNewBuyerIdThrowsGivenInvalidValue()
    {
        var basket = new Basket("buyer1");

        Assert.Throws<BasketDomainException>(() => basket.SetNewBuyerId(""));
    }
}
