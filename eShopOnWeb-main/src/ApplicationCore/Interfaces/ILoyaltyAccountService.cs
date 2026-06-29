using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface ILoyaltyAccountService
{
    Task<LoyaltyAccount> GetOrCreateAccountAsync(string customerId, CancellationToken cancellationToken = default);
    Task<LoyaltyAccount> AwardPointsAsync(string customerId, int points, string? orderId = null, CancellationToken cancellationToken = default);
    Task<LoyaltyAccount> SpendPointsAsync(string customerId, int points, string? orderId = null, CancellationToken cancellationToken = default);
    Task<int> GetBalanceAsync(string customerId, CancellationToken cancellationToken = default);
}
