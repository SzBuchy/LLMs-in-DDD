using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services;

public class BasketViewModelService : IBasketViewModelService
{
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IUriComposer _uriComposer;

    public BasketViewModelService(
        IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IUriComposer uriComposer)
    {
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
        _uriComposer = uriComposer;
    }

    public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
    {
        var spec = new BasketWithItemsSpecification(userName);
        var basket = await _basketRepository.GetBySpecAsync(spec);

        if (basket == null)
        {
            basket = new Basket { BuyerId = userName };
            await _basketRepository.AddAsync(basket);
        }

        var viewModel = new BasketViewModel
        {
            Id = basket.Id,
            BuyerId = basket.BuyerId,
            Items = await MapBasketItems(basket.Items)
        };

        return viewModel;
    }

    private async Task<List<BasketItemViewModel>> MapBasketItems(IReadOnlyCollection<BasketItem> basketItems)
    {
        var viewModels = new List<BasketItemViewModel>();
        foreach (var item in basketItems)
        {
            var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
            if (catalogItem == null) continue;

            viewModels.Add(new BasketItemViewModel
            {
                Id = item.Id,
                CatalogItemId = item.CatalogItemId,
                ProductName = catalogItem.Name,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                PictureUri = _uriComposer.ComposePicUri(catalogItem.PictureUri)
            });
        }
        return viewModels;
    }
}
