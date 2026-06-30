using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Flame.Common.Core.Exceptions;
using VOEConsulting.Infrastructure.Persistence;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly BasketAppDbContext _dbContext;
        private readonly IMapper _mapper;
        public BasketRepository(BasketAppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddAsync(Basket basket, CancellationToken cancellationToken)
        {
            var basketEntity = _mapper.Map<BasketEntity>(basket);
            await _dbContext.AddAsync(basketEntity);
        }



        public Task DeleteAsync(Guid id)
        {
            return Task.FromResult(_dbContext.Remove(id));
        }

        public Task<IEnumerable<Basket>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Basket?> GetByIdAsync(Guid id)
        {
            var basketEntity = await _dbContext.Baskets
                             .Include(b => b.Customer)
                             .Include(b => b.Coupon)
                             .Include(b => b.BasketItems)
                                 .ThenInclude(bi => bi.Seller)
                             .Where(b => b.Id == id)
                             .FirstOrDefaultAsync();

            var basket = _mapper.Map<Basket>(basketEntity);
            basket?.ClearEvents();
            return basket;
        }

        private async Task<BasketEntity> GetByBasketEntityIdAsync(Guid basketId)
        {
            // Retrieve the basket including its items and associated sellers
            return await _dbContext.Baskets
                .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Seller)
                .FirstOrDefaultAsync(b => b.Id == basketId)
                ?? throw new InvalidOperationException("Basket not found.");
        }

        public async Task UpdateAsync(Basket basket)
        {
            var existingEntity = await _dbContext.Baskets
                .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Seller)
                .FirstOrDefaultAsync(b => b.Id == basket.Id.Value);

            if (existingEntity == null)
            {
                throw new FlameApplicationException("Basket not found.");
            }

            existingEntity.TaxPercentage = basket.TaxPercentage;
            existingEntity.TotalAmount = basket.TotalAmount;
            existingEntity.CouponId = basket.CouponId;
            existingEntity.CustomerId = basket.Customer.Id;

            var currentItemEntities = existingEntity.BasketItems.ToList();
            var domainItems = basket.BasketItems.SelectMany(kvp => kvp.Value.Items).ToList();

            // 1. Remove items no longer in domain
            var domainItemIds = domainItems.Select(di => (Guid)di.Id).ToHashSet();
            foreach (var existingItem in currentItemEntities)
            {
                if (!domainItemIds.Contains(existingItem.Id))
                {
                    _dbContext.Remove(existingItem);
                }
            }

            // 2. Add or update items
            foreach (var domainItem in domainItems)
            {
                var existingItem = currentItemEntities.FirstOrDefault(ei => ei.Id == (Guid)domainItem.Id);
                if (existingItem != null)
                {
                    existingItem.QuantityValue = domainItem.Quantity.Value;
                    existingItem.QuantityLimit = domainItem.Quantity.Limit;
                    existingItem.PricePerUnit = domainItem.Quantity.PricePerUnit;
                    existingItem.Name = domainItem.Name;
                    existingItem.ImageUrl = domainItem.ImageUrl;
                }
                else
                {
                    var newItemEntity = _mapper.Map<BasketItemEntity>(domainItem);
                    newItemEntity.BasketId = basket.Id.Value;

                    var existingSeller = await _dbContext.Set<SellerEntity>()
                        .FirstOrDefaultAsync(s => s.Name == domainItem.Seller.Name);

                    if (existingSeller != null)
                    {
                        newItemEntity.SellerId = existingSeller.Id;
                        newItemEntity.Seller = null;
                    }
                    else
                    {
                        newItemEntity.SellerId = domainItem.Seller.Id;
                        _dbContext.Attach(newItemEntity.Seller);
                    }

                    await _dbContext.AddAsync(newItemEntity);
                }
            }
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            return (await _dbContext.Baskets.FirstOrDefaultAsync(x => x.Id == id)) != null;
        }

        public async Task<bool> IsExistByCustomerIdAsync(Guid customerId)
        {
            return (await _dbContext.Baskets.FirstOrDefaultAsync(x => x.CustomerId == customerId)) != null;
        }
    }
}
