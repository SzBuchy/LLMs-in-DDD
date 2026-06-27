using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;

namespace VOEConsulting.Flame.BasketContext.Application.Repositories
{
    public interface ILoyaltyAccountRepository : IRepository<LoyaltyAccount>
    {
        Task<LoyaltyAccount?> GetByCustomerIdAsync(Guid customerId);
        Task<bool> IsExistByCustomerIdAsync(Guid customerId);
    }
}
