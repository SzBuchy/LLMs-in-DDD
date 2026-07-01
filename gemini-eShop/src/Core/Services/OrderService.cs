using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;

namespace Core.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;

    public OrderService(IRepository<Order> orderRepository,
                        IRepository<Basket> basketRepository,
                        IRepository<CatalogItem> itemRepository)
    {
        _orderRepository = orderRepository;
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public async Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.GetBySpecAsync(basketSpec);
        if (basket == null) return;

        var items = new List<OrderItem>();
        foreach (var basketItem in basket.Items)
        {
            var catalogItem = await _itemRepository.GetByIdAsync(basketItem.CatalogItemId);
            if (catalogItem == null) continue;

            var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, catalogItem.PictureUri);
            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                UnitPrice = basketItem.UnitPrice,
                Units = basketItem.Quantity
            };
            items.Add(orderItem);
        }

        var order = new Order
        {
            BuyerId = basket.BuyerId,
            ShipToAddress = shippingAddress
        };
        foreach (var item in items)
        {
            order.AddOrderItem(item);
        }

        await _orderRepository.AddAsync(order);
        await _basketRepository.DeleteAsync(basket);
    }
}
