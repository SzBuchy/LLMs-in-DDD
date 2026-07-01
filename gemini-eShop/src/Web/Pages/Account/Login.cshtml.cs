using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IBasketService _basketService;

    public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IBasketService basketService)
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

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
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
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
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
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return Page();
    }
}
