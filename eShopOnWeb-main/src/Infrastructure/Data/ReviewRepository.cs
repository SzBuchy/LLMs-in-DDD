using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class ReviewRepository : IReviewRepository
{
    private readonly CatalogContext _dbContext;

    public ReviewRepository(CatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        _dbContext.Reviews.Add(review);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return review;
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        _dbContext.Reviews.Update(review);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Review?> GetByIdAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Reviews
            .FirstOrDefaultAsync(review => review.Id == reviewId, cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> ListByCatalogItemIdAsync(int catalogItemId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .Where(review => review.CatalogItemId == catalogItemId)
            .ToListAsync(cancellationToken);
    }
}
