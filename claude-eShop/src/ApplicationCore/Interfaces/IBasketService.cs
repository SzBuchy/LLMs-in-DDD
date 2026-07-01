using ApplicationCore.Entities.BasketAggregate;

namespace ApplicationCore.Interfaces;

public interface IBasketService
{
    Task<Basket> AddItemToBasketAsync(string buyerId, int catalogItemId, decimal price, int quantity = 1);
    Task SetQuantitiesAsync(int basketId, IDictionary<int, int> quantities);
    Task DeleteBasketItemAsync(int basketId, int basketItemId);
    Task TransferBasketAsync(string anonymousId, string userId);
    Task DeleteBasketAsync(int basketId);
}
