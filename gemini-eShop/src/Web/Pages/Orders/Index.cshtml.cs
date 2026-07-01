using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.ViewModels;

namespace Web.Pages.Orders;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IRepository<Order> _orderRepository;

    public IndexModel(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public List<OrderViewModel> Orders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var buyerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(buyerId)) return RedirectToPage("/Index");

        var spec = new CustomerOrdersWithItemsSpecification(buyerId);
        var orders = await _orderRepository.ListAsync(spec);

        Orders = orders.Select(o => new OrderViewModel
        {
            OrderNumber = o.Id,
            OrderDate = o.OrderDate,
            Total = o.Total(),
            ShippingAddress = o.ShipToAddress
        }).ToList();

        return Page();
    }
}
