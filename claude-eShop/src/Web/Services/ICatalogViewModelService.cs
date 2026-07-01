using Web.ViewModels;

namespace Web.Services;

public interface ICatalogViewModelService
{
    Task<CatalogIndexViewModel> GetCatalogItemsAsync(int pageIndex, int itemsPage, int? brandId, int? typeId);
}
