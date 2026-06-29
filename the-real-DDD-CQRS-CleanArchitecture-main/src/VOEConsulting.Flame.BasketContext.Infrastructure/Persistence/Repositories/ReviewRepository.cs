using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Infrastructure.Persistence;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly BasketAppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ReviewRepository(BasketAppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> HasPublishedReviewForProductAsync(Id<Customer> customerId, Id<Product> productId, CancellationToken cancellationToken = default)
        {
            var customerGuid = customerId.Value;
            var productGuid = productId.Value;
            var publishedStatus = (int)ReviewStatus.Published;

            return await _dbContext.Reviews.AnyAsync(r =>
                r.CustomerId == customerGuid &&
                r.ProductId == productGuid &&
                r.Status == publishedStatus,
                cancellationToken);
        }

        public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<ReviewEntity>(review);
            await _dbContext.Reviews.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<ReviewEntity>(review);
            _dbContext.Reviews.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<Review?> GetByIdAsync(Id<Review> id, CancellationToken cancellationToken = default)
        {
            var guid = id.Value;
            var entity = await _dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == guid, cancellationToken);

            return entity != null ? _mapper.Map<Review>(entity) : null;
        }

        public async Task<IReadOnlyCollection<Review>> GetByProductIdAsync(Id<Product> productId, CancellationToken cancellationToken = default)
        {
            var productGuid = productId.Value;
            var entities = await _dbContext.Reviews
                .Where(r => r.ProductId == productGuid)
                .ToListAsync(cancellationToken);

            var domainModels = _mapper.Map<List<Review>>(entities);
            return domainModels.AsReadOnly();
        }
    }
}
