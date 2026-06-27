using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface ILoyaltyAccountRepository
{
    Task<LoyaltyAccount> AddAsync(LoyaltyAccount account);
    Task UpdateAsync(LoyaltyAccount account);
    Task<LoyaltyAccount?> GetByBuyerIdAsync(string buyerId);
}
