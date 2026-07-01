using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICatalogViewModelService _catalogViewModelService;
    private readonly IBasketService _basketService;

    public IndexModel(ICatalogViewModelService catalogViewModelService, IBasketService basketService)
    {
        _catalogViewModelService = catalogViewModelService;
        _basketService = basketService;
    }

    public CatalogIndexViewModel CatalogModel { get; set; } = new();

    public async Task OnGetAsync(int? brandFilterApplied, int? typeFilterApplied, int? pageId)
    {
        int itemsPage = 10;
        int actualPage = pageId ?? 0;
        CatalogModel = await _catalogViewModelService.GetCatalogItems(actualPage, itemsPage, brandFilterApplied, typeFilterApplied);
    }

    public async Task<IActionResult> OnPostAsync(int catalogItemId, decimal price)
    {
        var buyerId = GetBuyerId();
        await _basketService.AddItemToBasketAsync(buyerId, catalogItemId, price);
        return RedirectToPage();
    }

    private string GetBuyerId()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return User.Identity.Name!;
        }

        string? anonymousId = null;
        if (Request.Cookies.ContainsKey("eShop_anonymous_id"))
        {
            anonymousId = Request.Cookies["eShop_anonymous_id"];
        }

        if (string.IsNullOrEmpty(anonymousId))
        {
            anonymousId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTimeOffset.UtcNow.AddYears(1) };
            Response.Cookies.Append("eShop_anonymous_id", anonymousId, cookieOptions);
        }

        return anonymousId;
    }
}
