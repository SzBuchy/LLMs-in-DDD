using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class ReviewsByCatalogItemIdSpecification : Specification<Review>
{
    public ReviewsByCatalogItemIdSpecification(int catalogItemId)
    {
        Query.Where(r => r.CatalogItemId == catalogItemId);
    }
}
