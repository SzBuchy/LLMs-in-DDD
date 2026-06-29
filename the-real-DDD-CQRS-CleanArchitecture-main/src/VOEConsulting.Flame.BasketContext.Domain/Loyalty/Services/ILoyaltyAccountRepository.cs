using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Services
{
    public interface ILoyaltyAccountRepository
    {
        Task AddAsync(LoyaltyAccount account, CancellationToken cancellationToken = default);
        Task UpdateAsync(LoyaltyAccount account, CancellationToken cancellationToken = default);
        Task<LoyaltyAccount?> GetByIdAsync(Id<LoyaltyAccount> id, CancellationToken cancellationToken = default);
        Task<LoyaltyAccount?> GetByCustomerIdAsync(Id<Customer> customerId, CancellationToken cancellationToken = default);
    }
}
