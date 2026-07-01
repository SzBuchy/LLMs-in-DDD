using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IBasketService _basketService;

    public RegisterModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IBasketService basketService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _basketService = basketService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                if (Request.Cookies.ContainsKey("eShop_anonymous_id"))
                {
                    var anonymousId = Request.Cookies["eShop_anonymous_id"];
                    if (!string.IsNullOrEmpty(anonymousId))
                    {
                        await _basketService.TransferBasketAsync(anonymousId, Input.Email);
                        Response.Cookies.Delete("eShop_anonymous_id");
                    }
                }

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }
}
