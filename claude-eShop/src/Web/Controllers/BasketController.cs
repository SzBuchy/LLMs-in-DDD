using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers;

public class BasketController : Controller
{
    private readonly IBasketService _basketService;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IUriComposer _uriComposer;

    public BasketController(
        IBasketService basketService,
        IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IUriComposer uriComposer)
    {
        _basketService = basketService;
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
        _uriComposer = uriComposer;
    }

    public async Task<IActionResult> Index()
    {
        await TransferAnonymousBasketIfNeededAsync();

        var buyerId = HttpContext.GetOrCreateBuyerId();
        var vm = await GetBasketViewModelAsync(buyerId);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddToBasket(int catalogItemId, int quantity = 1)
    {
        var catalogItem = await _itemRepository.GetByIdAsync(catalogItemId);
        if (catalogItem == null)
        {
            return NotFound();
        }

        var buyerId = HttpContext.GetOrCreateBuyerId();
        await _basketService.AddItemToBasketAsync(buyerId, catalogItemId, catalogItem.Price, quantity);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SetQuantities(int basketId, Dictionary<int, int> quantities)
    {
        await _basketService.SetQuantitiesAsync(basketId, quantities);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem(int basketId, int basketItemId)
    {
        await _basketService.DeleteBasketItemAsync(basketId, basketItemId);
        return RedirectToAction(nameof(Index));
    }

    private async Task TransferAnonymousBasketIfNeededAsync()
    {
        if (HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var anonymousId = HttpContext.GetAnonymousBuyerIdCookie();
        if (string.IsNullOrEmpty(anonymousId))
        {
            return;
        }

        await _basketService.TransferBasketAsync(anonymousId, HttpContext.User.Identity.Name!);
        HttpContext.ClearAnonymousBuyerIdCookie();
    }

    private async Task<BasketViewModel> GetBasketViewModelAsync(string buyerId)
    {
        var spec = new BasketWithItemsSpecification(buyerId);
        var basket = await _basketRepository.FirstOrDefaultAsync(spec);

        if (basket == null)
        {
            return new BasketViewModel { BuyerId = buyerId };
        }

        var vm = new BasketViewModel { Id = basket.Id, BuyerId = basket.BuyerId };

        foreach (var item in basket.Items)
        {
            var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
            vm.Items.Add(new BasketItemViewModel
            {
                Id = item.Id,
                CatalogItemId = item.CatalogItemId,
                ProductName = catalogItem?.Name ?? "Unknown product",
                PictureUri = _uriComposer.ComposePicUri(catalogItem?.PictureUri ?? string.Empty),
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
            });
        }

        return vm;
    }
}
