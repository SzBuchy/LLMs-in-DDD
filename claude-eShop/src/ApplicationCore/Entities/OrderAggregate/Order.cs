using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities.OrderAggregate;

// Aggregate root. An order is a record of a completed purchase: it is created once with its
// full set of items and is never mutated afterwards - only read.
public class Order : BaseEntity, IAggregateRoot
{
    public string BuyerId { get; private set; }
    public DateTimeOffset OrderDate { get; private set; }
    public Address ShipToAddress { get; private set; }

    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order()
    {
        BuyerId = string.Empty;
        ShipToAddress = null!;
        _orderItems = new List<OrderItem>();
    }

    public Order(string buyerId, Address shipToAddress, List<OrderItem> items)
    {
        if (items == null || items.Count == 0)
        {
            throw new OrderDomainException("An order must contain at least one item");
        }

        BuyerId = buyerId;
        ShipToAddress = shipToAddress;
        _orderItems = items;
        OrderDate = DateTimeOffset.Now;
    }

    public decimal Total()
    {
        return _orderItems.Sum(item => item.UnitPrice * item.Units);
    }
}
