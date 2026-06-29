using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Web.Pages.Reviews;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IProductReviewService _productReviewService;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IUriComposer _uriComposer;

    public CreateModel(IProductReviewService productReviewService, IRepository<CatalogItem> itemRepository, IUriComposer uriComposer)
    {
        _productReviewService = productReviewService;
        _itemRepository = itemRepository;
        _uriComposer = uriComposer;
    }

    [BindProperty]
    public int ProductId { get; set; }

    [BindProperty]
    [Range(1, 5, ErrorMessage = "Ocena musi być w przedziale od 1 do 5.")]
    [Display(Name = "Ocena (1-5)")]
    public int Rating { get; set; } = 5;

    [BindProperty]
    [Required(ErrorMessage = "Treść recenzji jest wymagana.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Treść recenzji musi mieć od 10 do 500 znaków.")]
    [Display(Name = "Treść recenzji")]
    public string TextContent { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;
    public string ProductPictureUri { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int productId)
    {
        ProductId = productId;
        var product = await _itemRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return RedirectToPage("/Index");
        }

        ProductName = product.Name;
        ProductPictureUri = _uriComposer.ComposePicUri(product.PictureUri);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var product = await _itemRepository.GetByIdAsync(ProductId);
        if (product == null)
        {
            return RedirectToPage("/Index");
        }

        ProductName = product.Name;
        ProductPictureUri = _uriComposer.ComposePicUri(product.PictureUri);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var customerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(customerId))
        {
            ModelState.AddModelError(string.Empty, "Musisz być zalogowany, aby dodać recenzję.");
            return Page();
        }

        try
        {
            await _productReviewService.AddProductReviewAsync(customerId, ProductId, Rating, TextContent);
            TempData["SuccessMessage"] = "Recenzja została pomyślnie dodana i oczekuje na moderację.";
            return RedirectToPage("/Index");
        }
        catch (CatalogItemNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return Page();
    }
}
