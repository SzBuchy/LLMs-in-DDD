using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class ProductReviewRepository : EfRepository<ProductReview>, IProductReviewRepository
{
    private readonly CatalogContext _dbContext;

    public ProductReviewRepository(CatalogContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductReviews
            .Where(r => r.CatalogItemId == productId)
            .ToListAsync(cancellationToken);
    }
}
