using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces;

public interface IOrderService
{
    Task CreateOrderAsync(int basketId, Address shippingAddress);
}
