using VOEConsulting.Flame.BasketContext.Tests.Data;
using static VOEConsulting.Flame.BasketContext.Tests.Data.BasketData;
namespace VOEConsulting.Flame.BasketContext.Tests.Unit.Factories
{
    public static class BasketFactory
    {
        public static Basket Create(decimal? taxPercentage = null, Customer? customer = null)
        {
            var basket = Basket.Create(taxPercentage ?? TaxAmount, customer ?? BasketData.Customer);
            basket.ClearEvents();

            return basket;
        }
    }
}
