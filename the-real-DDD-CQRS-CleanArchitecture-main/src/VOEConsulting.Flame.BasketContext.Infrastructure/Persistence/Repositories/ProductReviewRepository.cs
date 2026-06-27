using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Infrastructure.Persistence;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly BasketAppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductReviewRepository(BasketAppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddAsync(ProductReview entity, CancellationToken cancellationToken)
        {
            var reviewEntity = _mapper.Map<ProductReviewEntity>(entity);
            await _dbContext.ProductReviews.AddAsync(reviewEntity, cancellationToken);
        }

        public async Task<ProductReview?> GetByIdAsync(Guid id)
        {
            var reviewEntity = await _dbContext.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id);

            return reviewEntity is null ? null : _mapper.Map<ProductReview>(reviewEntity);
        }

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId)
        {
            var reviewEntities = await _dbContext.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductReview>>(reviewEntities);
        }

        public async Task<IEnumerable<ProductReview>> GetAllAsync()
        {
            var reviewEntities = await _dbContext.ProductReviews.ToListAsync();
            return _mapper.Map<IEnumerable<ProductReview>>(reviewEntities);
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            return await _dbContext.ProductReviews.AnyAsync(r => r.Id == id);
        }

        public Task UpdateAsync(ProductReview entity)
        {
            var reviewEntity = _mapper.Map<ProductReviewEntity>(entity);
            _dbContext.ProductReviews.Update(reviewEntity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var reviewEntity = await _dbContext.ProductReviews.FirstOrDefaultAsync(r => r.Id == id);
            if (reviewEntity is not null)
                _dbContext.ProductReviews.Remove(reviewEntity);
        }
    }
}
