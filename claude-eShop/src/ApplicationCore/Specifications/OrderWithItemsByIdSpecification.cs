using ApplicationCore.Entities.OrderAggregate;

namespace ApplicationCore.Specifications;

public class OrderWithItemsByIdSpecification : BaseSpecification<Order>
{
    public OrderWithItemsByIdSpecification(int orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.OrderItems);
    }
}
