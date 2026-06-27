using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class ReviewRepository : IReviewRepository
{
    private readonly IRepository<Review> _repository;

    public ReviewRepository(IRepository<Review> repository)
    {
        _repository = repository;
    }

    public Task<Review> AddAsync(Review review) => _repository.AddAsync(review);

    public Task<Review?> GetByIdAsync(int reviewId) => _repository.GetByIdAsync(reviewId);

    public Task<List<Review>> GetByCatalogItemIdAsync(int catalogItemId) =>
        _repository.ListAsync(new ReviewsByCatalogItemIdSpecification(catalogItemId));
}
