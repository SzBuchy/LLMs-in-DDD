using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IReviewRepository
{
    Task<Review> AddAsync(Review review);
    Task<Review?> GetByIdAsync(int reviewId);
    Task<List<Review>> GetByCatalogItemIdAsync(int catalogItemId);
}
