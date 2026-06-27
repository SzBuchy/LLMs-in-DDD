using VOEConsulting.Flame.BasketContext.Domain.Loyalty;

namespace VOEConsulting.Flame.BasketContext.Application.Repositories
{
    public interface ILoyaltyAccountRepository : IRepository<LoyaltyAccount>
    {
        Task<LoyaltyAccount?> GetByCustomerIdAsync(Guid customerId);
        Task<bool> ExistsByCustomerIdAsync(Guid customerId);
    }
}
