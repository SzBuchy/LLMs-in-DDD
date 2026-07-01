using ApplicationCore.Entities.OrderAggregate;

namespace ApplicationCore.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(int basketId, Address shipToAddress);
}
