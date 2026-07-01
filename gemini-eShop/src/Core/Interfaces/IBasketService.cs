using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces;

public interface IBasketService
{
    Task TransferBasketAsync(string anonymousId, string userName);
    Task<Basket> AddItemToBasketAsync(string userName, int catalogItemId, decimal price, int quantity = 1);
    Task SetQuantitiesAsync(int basketId, Dictionary<string, int> quantities);
    Task DeleteBasketAsync(int basketId);
}
