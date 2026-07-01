using Core.Entities;

namespace Core.Specifications;

public class CustomerOrdersWithItemsSpecification : BaseSpecification<Order>
{
    public CustomerOrdersWithItemsSpecification(string buyerId)
        : base(o => o.BuyerId == buyerId)
    {
        AddInclude(o => o.OrderItems);
    }
}
