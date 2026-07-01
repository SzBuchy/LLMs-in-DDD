using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Basket;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly IBasketViewModelService _basketViewModelService;
    private readonly IOrderService _orderService;

    public CheckoutModel(IBasketViewModelService basketViewModelService, IOrderService orderService)
    {
        _basketViewModelService = basketViewModelService;
        _orderService = orderService;
    }

    [BindProperty]
    public ShippingAddressViewModel ShippingAddress { get; set; } = new();

    public BasketViewModel BasketModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var buyerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(buyerId)) return RedirectToPage("/Index");

        BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(buyerId);
        if (!BasketModel.Items.Any())
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var buyerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(buyerId)) return RedirectToPage("/Index");

        BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(buyerId);
        if (!ModelState.IsValid || !BasketModel.Items.Any())
        {
            return Page();
        }

        var address = new Address(
            ShippingAddress.Street,
            ShippingAddress.City,
            ShippingAddress.State,
            ShippingAddress.Country,
            ShippingAddress.ZipCode
        );

        await _orderService.CreateOrderAsync(BasketModel.Id, address);
        return RedirectToPage("Success");
    }
}

public class ShippingAddressViewModel
{
    [Required]
    public string Street { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string State { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Zip Code")]
    public string ZipCode { get; set; } = string.Empty;
}
