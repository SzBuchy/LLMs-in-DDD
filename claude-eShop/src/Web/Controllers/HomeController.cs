using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

public class HomeController : Controller
{
    private const int ItemsPerPage = 10;

    private readonly ICatalogViewModelService _catalogViewModelService;

    public HomeController(ICatalogViewModelService catalogViewModelService)
    {
        _catalogViewModelService = catalogViewModelService;
    }

    public async Task<IActionResult> Index(int pageIndex = 0, int? brandId = null, int? typeId = null)
    {
        var vm = await _catalogViewModelService.GetCatalogItemsAsync(pageIndex, ItemsPerPage, brandId, typeId);
        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
