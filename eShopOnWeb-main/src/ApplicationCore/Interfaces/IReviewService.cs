using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IReviewService
{
    Task<Review> AddReviewAsync(string buyerId, int catalogItemId, int rating, string content);
}
