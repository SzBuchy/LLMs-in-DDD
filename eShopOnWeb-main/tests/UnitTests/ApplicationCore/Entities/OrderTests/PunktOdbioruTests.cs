using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.OrderTests;

public class PunktOdbioruTests
{
    [Fact]
    public void ObjectsWithIdenticalValuesAreEqual()
    {
        var punkt1 = new PunktOdbioru("Paczkomat WAW01A", "Sezamkowa 10", "Warszawa", "00-001");
        var punkt2 = new PunktOdbioru("Paczkomat WAW01A", "Sezamkowa 10", "Warszawa", "00-001");

        Assert.Equal(punkt1, punkt2);
        Assert.True(punkt1 == punkt2);
        Assert.False(punkt1 != punkt2);
        Assert.True(punkt1.Equals(punkt2));
        Assert.Equal(punkt1.GetHashCode(), punkt2.GetHashCode());
    }

    [Fact]
    public void ObjectsWithDifferentValuesAreNotEqual()
    {
        var punkt1 = new PunktOdbioru("Paczkomat WAW01A", "Sezamkowa 10", "Warszawa", "00-001");
        var punkt2 = new PunktOdbioru("Paczkomat WAW02B", "Sezamkowa 10", "Warszawa", "00-001");
        var punkt3 = new PunktOdbioru("Paczkomat WAW01A", "Zielona 5", "Warszawa", "00-001");

        Assert.NotEqual(punkt1, punkt2);
        Assert.True(punkt1 != punkt2);
        Assert.False(punkt1 == punkt2);
        Assert.NotEqual(punkt1.GetHashCode(), punkt2.GetHashCode());

        Assert.NotEqual(punkt1, punkt3);
        Assert.True(punkt1 != punkt3);
        Assert.False(punkt1 == punkt3);
        Assert.NotEqual(punkt1.GetHashCode(), punkt3.GetHashCode());
    }

    [Theory]
    [InlineData("", "Sezamkowa 10", "Warszawa", "00-001")]
    [InlineData(null, "Sezamkowa 10", "Warszawa", "00-001")]
    [InlineData("Paczkomat WAW01A", "", "Warszawa", "00-001")]
    [InlineData("Paczkomat WAW01A", null, "Warszawa", "00-001")]
    [InlineData("Paczkomat WAW01A", "Sezamkowa 10", "", "00-001")]
    [InlineData("Paczkomat WAW01A", "Sezamkowa 10", null, "00-001")]
    [InlineData("Paczkomat WAW01A", "Sezamkowa 10", "Warszawa", "")]
    [InlineData("Paczkomat WAW01A", "Sezamkowa 10", "Warszawa", null)]
    public void ThrowsGivenNullOrEmptyValues(string? nazwaPunktu, string? ulica, string? miasto, string? kodPocztowy)
    {
        Assert.ThrowsAny<ArgumentException>(() => 
            new PunktOdbioru(nazwaPunktu!, ulica!, miasto!, kodPocztowy!));
    }
}
