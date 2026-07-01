using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IBasketService _basketService;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<Order> _orderRepository;

    public OrderController(
        IOrderService orderService,
        IBasketService basketService,
        IRepository<Basket> basketRepository,
        IRepository<Order> orderRepository)
    {
        _orderService = orderService;
        _basketService = basketService;
        _basketRepository = basketRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IActionResult> Checkout()
    {
        await TransferAnonymousBasketIfNeededAsync();
        var buyerId = HttpContext.GetOrCreateBuyerId();
        var basket = await _basketRepository.FirstOrDefaultAsync(new BasketWithItemsSpecification(buyerId));

        if (basket == null || basket.Items.Count == 0)
        {
            return RedirectToAction("Index", "Basket");
        }

        return View(new Address("1 Main St", "Redmond", "WA", "USA", "98052"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(string street, string city, string state, string country, string zipCode)
    {
        var buyerId = HttpContext.GetOrCreateBuyerId();
        var basket = await _basketRepository.FirstOrDefaultAsync(new BasketWithItemsSpecification(buyerId));

        if (basket == null || basket.Items.Count == 0)
        {
            return RedirectToAction("Index", "Basket");
        }

        var shipToAddress = new Address(street, city, state, country, zipCode);
        var order = await _orderService.CreateOrderAsync(basket.Id, shipToAddress);

        return RedirectToAction(nameof(Success), new { orderId = order.Id });
    }

    public async Task<IActionResult> Success(int orderId)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(new OrderWithItemsByIdSpecification(orderId));
        if (order == null || order.BuyerId != HttpContext.User.Identity!.Name)
        {
            return NotFound();
        }

        return View(ToViewModel(order));
    }

    public async Task<IActionResult> Index()
    {
        var buyerId = HttpContext.User.Identity!.Name!;
        var orders = await _orderRepository.ListAsync(new CustomerOrdersWithItemsSpecification(buyerId));
        return View(orders.Select(ToViewModel).ToList());
    }

    public async Task<IActionResult> Detail(int orderId)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(new OrderWithItemsByIdSpecification(orderId));
        if (order == null || order.BuyerId != HttpContext.User.Identity!.Name)
        {
            return NotFound();
        }

        return View(ToViewModel(order));
    }

    private async Task TransferAnonymousBasketIfNeededAsync()
    {
        var anonymousId = HttpContext.GetAnonymousBuyerIdCookie();
        if (string.IsNullOrEmpty(anonymousId))
        {
            return;
        }

        await _basketService.TransferBasketAsync(anonymousId, HttpContext.User.Identity!.Name!);
        HttpContext.ClearAnonymousBuyerIdCookie();
    }

    private static OrderViewModel ToViewModel(Order order)
    {
        return new OrderViewModel
        {
            OrderNumber = order.Id,
            OrderDate = order.OrderDate,
            ShippingAddress = $"{order.ShipToAddress.Street}, {order.ShipToAddress.City}, {order.ShipToAddress.State}, {order.ShipToAddress.Country} {order.ShipToAddress.ZipCode}",
            Total = order.Total(),
            OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                ProductName = oi.ItemOrdered.ProductName,
                PictureUri = oi.ItemOrdered.PictureUri,
                UnitPrice = oi.UnitPrice,
                Units = oi.Units,
            }).ToList(),
        };
    }
}
