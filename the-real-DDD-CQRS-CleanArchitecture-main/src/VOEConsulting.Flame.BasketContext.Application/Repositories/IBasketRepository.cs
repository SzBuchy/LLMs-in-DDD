using VOEConsulting.Flame.BasketContext.Domain.Baskets;

namespace VOEConsulting.Flame.BasketContext.Application.Repositories
{
    public interface IBasketRepository : IRepository<Basket> {

        Task<bool> IsExistByCustomerIdAsync(Guid id);
    }

}
