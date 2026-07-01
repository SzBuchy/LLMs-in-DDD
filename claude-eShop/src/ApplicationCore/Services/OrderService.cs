using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IUriComposer _uriComposer;

    public OrderService(
        IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IRepository<Order> orderRepository,
        IUriComposer uriComposer)
    {
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
        _orderRepository = orderRepository;
        _uriComposer = uriComposer;
    }

    public async Task<Order> CreateOrderAsync(int basketId, Address shipToAddress)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

        if (basket == null)
        {
            throw new OrderDomainException($"Basket with id {basketId} not found");
        }

        if (basket.Items.Count == 0)
        {
            throw new OrderDomainException("Cannot create an order from an empty basket");
        }

        var orderItems = new List<OrderItem>();
        foreach (var basketItem in basket.Items)
        {
            var catalogItem = await _itemRepository.GetByIdAsync(basketItem.CatalogItemId);
            if (catalogItem == null)
            {
                throw new OrderDomainException($"Catalog item {basketItem.CatalogItemId} not found");
            }

            var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, _uriComposer.ComposePicUri(catalogItem.PictureUri));
            var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);
            orderItems.Add(orderItem);
        }

        var order = new Order(basket.BuyerId, shipToAddress, orderItems);
        return await _orderRepository.AddAsync(order);
    }
}
