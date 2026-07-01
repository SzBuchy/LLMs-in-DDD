using System;
using System.Collections.Generic;
using Core.Entities;

namespace Web.ViewModels;

public class OrderViewModel
{
    public int OrderNumber { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "Submitted";
    public Address ShippingAddress { get; set; } = null!;
    public List<OrderItemViewModel> OrderItems { get; set; } = new();
}
