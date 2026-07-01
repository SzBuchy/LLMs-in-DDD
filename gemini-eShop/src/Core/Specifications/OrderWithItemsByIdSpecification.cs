using Core.Entities;

namespace Core.Specifications;

public class OrderWithItemsByIdSpecification : BaseSpecification<Order>
{
    public OrderWithItemsByIdSpecification(int orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.OrderItems);
    }
}
