using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IReviewRepository
{
    Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default);

    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);

    Task<Review?> GetByIdAsync(int reviewId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Review>> ListByCatalogItemIdAsync(int catalogItemId, CancellationToken cancellationToken = default);
}
