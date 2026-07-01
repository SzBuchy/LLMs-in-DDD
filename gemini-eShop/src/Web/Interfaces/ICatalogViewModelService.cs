using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;

namespace Web.Interfaces;

public interface ICatalogViewModelService
{
    Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId);
    Task<IEnumerable<SelectListItem>> GetBrands();
    Task<IEnumerable<SelectListItem>> GetTypes();
}
