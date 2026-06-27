using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class LoyaltyAccountByBuyerIdSpecification : Specification<LoyaltyAccount>
{
    public LoyaltyAccountByBuyerIdSpecification(string buyerId)
    {
        Query
            .Where(account => account.BuyerId == buyerId)
            .Include(account => account.PointGrants)
            .Include(account => account.Redemptions);
    }
}
