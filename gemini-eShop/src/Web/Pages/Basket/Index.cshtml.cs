using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Basket;

public class IndexModel : PageModel
{
    private readonly IBasketViewModelService _basketViewModelService;
    private readonly IBasketService _basketService;

    public IndexModel(IBasketViewModelService basketViewModelService, IBasketService basketService)
    {
        _basketViewModelService = basketViewModelService;
        _basketService = basketService;
    }

    public BasketViewModel BasketModel { get; set; } = new();

    public async Task OnGetAsync()
    {
        var buyerId = GetBuyerId();
        BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(buyerId);
    }

    public async Task<IActionResult> OnPostUpdateAsync(Dictionary<string, int> items)
    {
        var buyerId = GetBuyerId();
        var basket = await _basketViewModelService.GetOrCreateBasketForUser(buyerId);
        await _basketService.SetQuantitiesAsync(basket.Id, items);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage("/Account/Login", new { returnUrl = "/Basket/Checkout" });
        }
        return RedirectToPage("Checkout");
    }

    private string GetBuyerId()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return User.Identity.Name!;
        }

        if (Request.Cookies.ContainsKey("eShop_anonymous_id"))
        {
            return Request.Cookies["eShop_anonymous_id"]!;
        }

        var anonymousId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTimeOffset.UtcNow.AddYears(1) };
        Response.Cookies.Append("eShop_anonymous_id", anonymousId, cookieOptions);
        return anonymousId;
    }
}
