using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Interfaces;
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

    public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
    {
        var filterSpec = new CatalogFilterSpecification(brandId, typeId);
        var totalItems = await _itemRepository.CountAsync(filterSpec);

        var pagedSpec = new CatalogFilterPaginatedSpecification(pageIndex * itemsPage, itemsPage, brandId, typeId);
        var items = await _itemRepository.ListAsync(pagedSpec);

        var viewModel = new CatalogIndexViewModel
        {
            CatalogItems = items.Select(i => new CatalogItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                PictureUri = _uriComposer.ComposePicUri(i.PictureUri)
            }).ToList(),
            Brands = (await GetBrands()).ToList(),
            Types = (await GetTypes()).ToList(),
            BrandFilterApplied = brandId,
            TypeFilterApplied = typeId,
            PaginationInfo = new PaginationInfoViewModel
            {
                ActualPage = pageIndex,
                ItemsPerPage = items.Count,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(((decimal)totalItems / itemsPage)),
                Previous = pageIndex > 0 ? "Previous" : "",
                Next = (pageIndex + 1) * itemsPage < totalItems ? "Next" : ""
            }
        };

        // Preserve current selections in brand/type lists
        if (brandId.HasValue)
        {
            var selected = viewModel.Brands.FirstOrDefault(b => b.Value == brandId.Value.ToString());
            if (selected != null) selected.Selected = true;
        }
        if (typeId.HasValue)
        {
            var selected = viewModel.Types.FirstOrDefault(t => t.Value == typeId.Value.ToString());
            if (selected != null) selected.Selected = true;
        }

        return viewModel;
    }

    public async Task<IEnumerable<SelectListItem>> GetBrands()
    {
        var brands = await _brandRepository.ListAllAsync();
        var items = brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Brand }).ToList();
        var allItem = new SelectListItem { Value = "", Text = "All Brands" };
        items.Insert(0, allItem);
        return items;
    }

    public async Task<IEnumerable<SelectListItem>> GetTypes()
    {
        var types = await _typeRepository.ListAllAsync();
        var items = types.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Type }).ToList();
        var allItem = new SelectListItem { Value = "", Text = "All Types" };
        items.Insert(0, allItem);
        return items;
    }
}
