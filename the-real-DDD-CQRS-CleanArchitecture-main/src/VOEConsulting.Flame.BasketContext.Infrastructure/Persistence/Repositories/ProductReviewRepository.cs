using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;
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
            var productReviewEntity = _mapper.Map<ProductReviewEntity>(entity);
            await _dbContext.ProductReviews.AddAsync(productReviewEntity, cancellationToken);
        }

        public async Task<ProductReview?> GetByIdAsync(Guid id)
        {
            var productReviewEntity = await _dbContext.ProductReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(review => review.Id == id);

            return _mapper.Map<ProductReview?>(productReviewEntity);
        }

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId)
        {
            var productReviewEntities = await _dbContext.ProductReviews
                .AsNoTracking()
                .Where(review => review.ProductId == productId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductReview>>(productReviewEntities);
        }

        public async Task<IEnumerable<ProductReview>> GetAllAsync()
        {
            var productReviewEntities = await _dbContext.ProductReviews
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductReview>>(productReviewEntities);
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            return await _dbContext.ProductReviews.AnyAsync(review => review.Id == id);
        }

        public async Task UpdateAsync(ProductReview entity)
        {
            var productReviewEntity = _mapper.Map<ProductReviewEntity>(entity);
            _dbContext.ProductReviews.Update(productReviewEntity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var productReviewEntity = await _dbContext.ProductReviews.FindAsync(id);

            if (productReviewEntity is not null)
            {
                _dbContext.ProductReviews.Remove(productReviewEntity);
            }
        }
    }
}
