using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface ILoyaltyAccountService
{
    Task<LoyaltyAccount> GetOrCreateAccountAsync(string buyerId);
    Task EarnPointsAsync(string buyerId, int points);
    Task<decimal> RedeemPointsAsync(string buyerId, int points);
}
