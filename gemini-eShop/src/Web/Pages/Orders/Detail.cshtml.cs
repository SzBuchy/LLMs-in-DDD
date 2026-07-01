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
public class DetailModel : PageModel
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUriComposer _uriComposer;

    public DetailModel(IRepository<Order> orderRepository, IUriComposer uriComposer)
    {
        _orderRepository = orderRepository;
        _uriComposer = uriComposer;
    }

    public OrderViewModel OrderDetails { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int orderId)
    {
        var buyerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(buyerId)) return RedirectToPage("/Index");

        var spec = new OrderWithItemsByIdSpecification(orderId);
        var order = await _orderRepository.GetBySpecAsync(spec);
        if (order == null || order.BuyerId != buyerId)
        {
            return RedirectToPage("Index");
        }

        OrderDetails = new OrderViewModel
        {
            OrderNumber = order.Id,
            OrderDate = order.OrderDate,
            Total = order.Total(),
            ShippingAddress = order.ShipToAddress,
            OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                CatalogItemId = oi.ItemOrdered.CatalogItemId,
                ProductName = oi.ItemOrdered.ProductName,
                UnitPrice = oi.UnitPrice,
                Units = oi.Units,
                PictureUri = _uriComposer.ComposePicUri(oi.ItemOrdered.PictureUri)
            }).ToList()
        };

        return Page();
    }
}
