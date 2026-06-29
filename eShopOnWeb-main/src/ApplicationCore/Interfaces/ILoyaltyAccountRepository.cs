using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface ILoyaltyAccountRepository : IRepository<LoyaltyAccount>
{
    Task<LoyaltyAccount?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
}
