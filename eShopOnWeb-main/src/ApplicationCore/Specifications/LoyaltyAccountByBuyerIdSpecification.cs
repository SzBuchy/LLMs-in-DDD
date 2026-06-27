using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class LoyaltyAccountByBuyerIdSpecification : Specification<LoyaltyAccount>
{
    public LoyaltyAccountByBuyerIdSpecification(string buyerId)
    {
        Query.Where(a => a.BuyerId == buyerId)
            .Include(a => a.PointsLots);
    }
}
