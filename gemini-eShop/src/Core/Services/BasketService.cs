using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;

namespace Core.Services;

public class BasketService : IBasketService
{
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;

    public BasketService(IRepository<Basket> basketRepository, IRepository<CatalogItem> itemRepository)
    {
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Basket> AddItemToBasketAsync(string userName, int catalogItemId, decimal price, int quantity = 1)
    {
        var spec = new BasketWithItemsSpecification(userName);
        var basket = await _basketRepository.GetBySpecAsync(spec);

        if (basket == null)
        {
            basket = new Basket { BuyerId = userName };
            await _basketRepository.AddAsync(basket);
        }

        basket.AddItem(catalogItemId, price, quantity);
        await _basketRepository.UpdateAsync(basket);
        return basket;
    }

    public async Task DeleteBasketAsync(int basketId)
    {
        var basket = await _basketRepository.GetByIdAsync(basketId);
        if (basket != null)
        {
            await _basketRepository.DeleteAsync(basket);
        }
    }

    public async Task SetQuantitiesAsync(int basketId, Dictionary<string, int> quantities)
    {
        var spec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.GetBySpecAsync(spec);
        if (basket == null) return;

        foreach (var item in basket.Items)
        {
            if (quantities.TryGetValue(item.CatalogItemId.ToString(), out var quantity))
            {
                basket.UpdateQuantity(item.CatalogItemId, quantity);
            }
        }
        basket.RemoveEmptyItems();
        await _basketRepository.UpdateAsync(basket);
    }

    public async Task TransferBasketAsync(string anonymousId, string userName)
    {
        var anonymousSpec = new BasketWithItemsSpecification(anonymousId);
        var anonymousBasket = await _basketRepository.GetBySpecAsync(anonymousSpec);
        if (anonymousBasket == null) return;

        var userSpec = new BasketWithItemsSpecification(userName);
        var userBasket = await _basketRepository.GetBySpecAsync(userSpec);

        if (userBasket == null)
        {
            userBasket = new Basket { BuyerId = userName };
            await _basketRepository.AddAsync(userBasket);
        }

        foreach (var item in anonymousBasket.Items)
        {
            userBasket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity);
        }

        await _basketRepository.UpdateAsync(userBasket);
        await _basketRepository.DeleteAsync(anonymousBasket);
    }
}
