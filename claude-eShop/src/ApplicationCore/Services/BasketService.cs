using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public class BasketService : IBasketService
{
    private readonly IRepository<Basket> _basketRepository;
    private readonly IAppLogger<BasketService> _logger;

    public BasketService(IRepository<Basket> basketRepository, IAppLogger<BasketService> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    public async Task<Basket> AddItemToBasketAsync(string buyerId, int catalogItemId, decimal price, int quantity = 1)
    {
        var spec = new BasketWithItemsSpecification(buyerId);
        var basket = await _basketRepository.FirstOrDefaultAsync(spec);

        if (basket == null)
        {
            basket = new Basket(buyerId);
            basket = await _basketRepository.AddAsync(basket);
        }

        basket.AddItem(catalogItemId, price, quantity);

        await _basketRepository.UpdateAsync(basket);
        return basket;
    }

    public async Task SetQuantitiesAsync(int basketId, IDictionary<int, int> quantities)
    {
        var spec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(spec);

        if (basket == null)
        {
            throw new BasketDomainException($"Basket with id {basketId} not found");
        }

        basket.SetQuantities(quantities);
        await _basketRepository.UpdateAsync(basket);
    }

    public async Task DeleteBasketItemAsync(int basketId, int basketItemId)
    {
        var spec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(spec);

        if (basket == null)
        {
            throw new BasketDomainException($"Basket with id {basketId} not found");
        }

        basket.SetQuantities(new Dictionary<int, int> { [basketItemId] = 0 });
        await _basketRepository.UpdateAsync(basket);
    }

    public async Task TransferBasketAsync(string anonymousId, string userId)
    {
        var anonymousBasketSpec = new BasketWithItemsSpecification(anonymousId);
        var anonymousBasket = await _basketRepository.FirstOrDefaultAsync(anonymousBasketSpec);

        if (anonymousBasket == null)
        {
            return;
        }

        var userBasketSpec = new BasketWithItemsSpecification(userId);
        var userBasket = await _basketRepository.FirstOrDefaultAsync(userBasketSpec);

        if (userBasket == null)
        {
            anonymousBasket.SetNewBuyerId(userId);
            await _basketRepository.UpdateAsync(anonymousBasket);
            _logger.LogInformation("Transferred anonymous basket {0} to buyer {1}", anonymousId, userId);
            return;
        }

        foreach (var item in anonymousBasket.Items)
        {
            userBasket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity);
        }

        await _basketRepository.UpdateAsync(userBasket);
        await _basketRepository.DeleteAsync(anonymousBasket);
    }

    public async Task DeleteBasketAsync(int basketId)
    {
        var basket = await _basketRepository.GetByIdAsync(basketId);
        if (basket != null)
        {
            await _basketRepository.DeleteAsync(basket);
        }
    }
}
