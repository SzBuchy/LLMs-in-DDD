using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.ViewModels;

public class CatalogIndexViewModel
{
    public List<CatalogItemViewModel> CatalogItems { get; set; } = new();
    public List<SelectListItem> Brands { get; set; } = new();
    public List<SelectListItem> Types { get; set; } = new();
    public int? BrandFilterApplied { get; set; }
    public int? TypeFilterApplied { get; set; }
    public PaginationInfoViewModel PaginationInfo { get; set; } = new();
}
