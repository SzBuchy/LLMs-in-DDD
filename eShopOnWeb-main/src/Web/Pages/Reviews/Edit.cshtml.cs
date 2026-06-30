using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Web.Pages.Reviews;

[Authorize]
public class EditModel : PageModel
{
    private readonly IProductReviewService _productReviewService;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IUriComposer _uriComposer;

    public EditModel(IProductReviewService productReviewService, IRepository<CatalogItem> itemRepository, IUriComposer uriComposer)
    {
        _productReviewService = productReviewService;
        _itemRepository = itemRepository;
        _uriComposer = uriComposer;
    }

    [BindProperty]
    public int ReviewId { get; set; }

    [BindProperty]
    public int ProductId { get; set; }

    [BindProperty]
    [Range(1, 5, ErrorMessage = "Ocena musi być w przedziale od 1 do 5.")]
    [Display(Name = "Ocena (1-5)")]
    public int Rating { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Treść recenzji jest wymagana.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Treść recenzji musi mieć od 10 do 500 znaków.")]
    [Display(Name = "Treść recenzji")]
    public string TextContent { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;
    public string ProductPictureUri { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int reviewId)
    {
        var customerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(customerId))
        {
            return Challenge();
        }

        var review = await _productReviewService.GetReviewByIdAsync(reviewId);
        if (review == null)
        {
            return RedirectToPage("/Index");
        }

        if (review.CustomerId != customerId)
        {
            return Forbid();
        }

        if (review.Status != ReviewStatus.Published)
        {
            TempData["ErrorMessage"] = "Tylko opublikowane recenzje mogą być edytowane.";
            return RedirectToPage("/Index");
        }

        ReviewId = review.Id;
        ProductId = review.CatalogItemId;
        Rating = review.Rating;
        TextContent = review.TextContent;

        var product = await _itemRepository.GetByIdAsync(review.CatalogItemId);
        if (product != null)
        {
            ProductName = product.Name;
            ProductPictureUri = _uriComposer.ComposePicUri(product.PictureUri);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var customerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(customerId))
        {
            return Challenge();
        }

        var review = await _productReviewService.GetReviewByIdAsync(ReviewId);
        if (review == null)
        {
            return RedirectToPage("/Index");
        }

        if (review.CustomerId != customerId)
        {
            return Forbid();
        }

        var product = await _itemRepository.GetByIdAsync(ProductId);
        if (product != null)
        {
            ProductName = product.Name;
            ProductPictureUri = _uriComposer.ComposePicUri(product.PictureUri);
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _productReviewService.EditProductReviewAsync(customerId, ReviewId, Rating, TextContent);
            TempData["SuccessMessage"] = "Recenzja została pomyślnie zaktualizowana i oczekuje na ponowną moderację.";
            return RedirectToPage("/Index");
        }
        catch (ProductReviewNotFoundException ex)
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
