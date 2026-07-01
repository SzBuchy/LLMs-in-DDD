using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;

namespace Web.Services;

public class CatalogViewModelService : ICatalogViewModelService
{
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IRepository<CatalogBrand> _brandRepository;
    private readonly IRepository<CatalogType> _typeRepository;
    private readonly IUriComposer _uriComposer;

    public CatalogViewModelService(
        IRepository<CatalogItem> itemRepository,
        IRepository<CatalogBrand> brandRepository,
        IRepository<CatalogType> typeRepository,
        IUriComposer uriComposer)
    {
        _itemRepository = itemRepository;
        _brandRepository = brandRepository;
        _typeRepository = typeRepository;
        _uriComposer = uriComposer;
    }

    public async Task<CatalogIndexViewModel> GetCatalogItemsAsync(int pageIndex, int itemsPage, int? brandId, int? typeId)
    {
        var filterSpec = new CatalogFilterSpecification(brandId, typeId);
        var pagedSpec = new CatalogFilterPaginatedSpecification(pageIndex * itemsPage, itemsPage, brandId, typeId);

        var items = await _itemRepository.ListAsync(pagedSpec);
        var totalItems = await _itemRepository.CountAsync(filterSpec);

        var vm = new CatalogIndexViewModel
        {
            CatalogItems = items.Select(i => new CatalogItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                PictureUri = _uriComposer.ComposePicUri(i.PictureUri),
                CatalogBrandId = i.CatalogBrandId,
                CatalogBrandName = i.CatalogBrand?.Brand,
                CatalogTypeId = i.CatalogTypeId,
                CatalogTypeName = i.CatalogType?.Type,
            }).ToList(),
            Brands = await GetBrandsAsync(),
            Types = await GetTypesAsync(),
            BrandFilterApplied = brandId,
            TypesFilterApplied = typeId,
            PaginationInfo = new PaginationInfoViewModel
            {
                ActualPage = pageIndex,
                ItemsPerPage = items.Count,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / itemsPage),
            },
        };

        return vm;
    }

    private async Task<IEnumerable<SelectListItem>> GetBrandsAsync()
    {
        var brands = await _brandRepository.ListAllAsync();
        var items = brands
            .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Brand })
            .ToList();
        items.Insert(0, new SelectListItem { Value = null, Text = "All", Selected = true });
        return items;
    }

    private async Task<IEnumerable<SelectListItem>> GetTypesAsync()
    {
        var types = await _typeRepository.ListAllAsync();
        var items = types
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Type })
            .ToList();
        items.Insert(0, new SelectListItem { Value = null, Text = "All", Selected = true });
        return items;
    }
}
