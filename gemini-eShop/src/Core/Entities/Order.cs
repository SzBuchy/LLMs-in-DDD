using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Entities;

public class Order : BaseEntity
{
    public string BuyerId { get; set; } = string.Empty;
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
    public Address ShipToAddress { get; set; } = null!;
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public decimal Total()
    {
        return _orderItems.Sum(item => item.UnitPrice * item.Units);
    }

    public void AddOrderItem(OrderItem item)
    {
        _orderItems.Add(item);
    }
}
