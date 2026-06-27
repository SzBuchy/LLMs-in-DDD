using Microsoft.EntityFrameworkCore;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Infrastructure.Persistence;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Repositories
{
    public class LoyaltyAccountRepository : ILoyaltyAccountRepository
    {
        private readonly BasketAppDbContext _dbContext;

        public LoyaltyAccountRepository(BasketAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(LoyaltyAccount entity, CancellationToken cancellationToken)
        {
            await _dbContext.LoyaltyAccounts.AddAsync(MapToEntity(entity), cancellationToken);
        }

        public async Task<LoyaltyAccount?> GetByIdAsync(Guid id)
        {
            var entity = await GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.Id == id);

            return entity is null ? null : MapToDomain(entity);
        }

        public async Task<LoyaltyAccount?> GetByCustomerIdAsync(Guid customerId)
        {
            var entity = await GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.CustomerId == customerId);

            return entity is null ? null : MapToDomain(entity);
        }

        public async Task<IEnumerable<LoyaltyAccount>> GetAllAsync()
        {
            var entities = await GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(MapToDomain);
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            return await _dbContext.LoyaltyAccounts.AnyAsync(account => account.Id == id);
        }

        public async Task<bool> IsExistByCustomerIdAsync(Guid customerId)
        {
            return await _dbContext.LoyaltyAccounts.AnyAsync(account => account.CustomerId == customerId);
        }

        public async Task UpdateAsync(LoyaltyAccount entity)
        {
            var existingEntity = await GetQueryable()
                .FirstOrDefaultAsync(account => account.Id == entity.Id);

            if (existingEntity is null)
                return;

            existingEntity.MaxPointsPerRedemption = entity.MaxPointsPerRedemption;
            SyncPointBatches(existingEntity, entity);
            SyncRedemptions(existingEntity, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbContext.LoyaltyAccounts.FindAsync(id);
            if (entity is not null)
                _dbContext.LoyaltyAccounts.Remove(entity);
        }

        private IQueryable<LoyaltyAccountEntity> GetQueryable()
        {
            return _dbContext.LoyaltyAccounts
                .Include(account => account.PointBatches)
                .Include(account => account.Redemptions);
        }

        private static LoyaltyAccountEntity MapToEntity(LoyaltyAccount account)
        {
            return new LoyaltyAccountEntity
            {
                Id = account.Id,
                CustomerId = account.CustomerId,
                MaxPointsPerRedemption = account.MaxPointsPerRedemption,
                PointBatches = account.PointBatches.Select(batch => new LoyaltyPointBatchEntity
                {
                    Id = batch.Id,
                    LoyaltyAccountId = account.Id,
                    OrderId = batch.OrderId,
                    Points = batch.Points,
                    RedeemedPoints = batch.RedeemedPoints,
                    AwardedAtUtc = batch.AwardedAtUtc,
                    ExpiresAtUtc = batch.ExpiresAtUtc,
                    ExpiredAtUtc = batch.ExpiredAtUtc
                }).ToList(),
                Redemptions = account.Redemptions.Select(redemption => new LoyaltyRedemptionEntity
                {
                    Id = redemption.Id,
                    LoyaltyAccountId = account.Id,
                    OrderId = redemption.OrderId,
                    Points = redemption.Points,
                    DiscountAmount = redemption.DiscountAmount,
                    RedeemedAtUtc = redemption.RedeemedAtUtc
                }).ToList()
            };
        }

        private static LoyaltyAccount MapToDomain(LoyaltyAccountEntity entity)
        {
            var pointBatches = entity.PointBatches
                .Select(batch => LoyaltyPointBatch.Restore(
                    batch.Id,
                    batch.OrderId,
                    batch.Points,
                    batch.RedeemedPoints,
                    batch.AwardedAtUtc,
                    batch.ExpiresAtUtc,
                    batch.ExpiredAtUtc));

            var redemptions = entity.Redemptions
                .Select(redemption => LoyaltyRedemption.Restore(
                    redemption.Id,
                    redemption.OrderId,
                    redemption.Points,
                    redemption.DiscountAmount,
                    redemption.RedeemedAtUtc));

            return LoyaltyAccount.Restore(
                entity.Id,
                Id<Customer>.FromGuid(entity.CustomerId),
                entity.MaxPointsPerRedemption,
                pointBatches,
                redemptions);
        }

        private static void SyncPointBatches(LoyaltyAccountEntity existingEntity, LoyaltyAccount account)
        {
            foreach (var batch in account.PointBatches)
            {
                var existingBatch = existingEntity.PointBatches.SingleOrDefault(entity => entity.Id == batch.Id);
                if (existingBatch is null)
                {
                    existingEntity.PointBatches.Add(new LoyaltyPointBatchEntity
                    {
                        Id = batch.Id,
                        LoyaltyAccountId = account.Id,
                        OrderId = batch.OrderId,
                        Points = batch.Points,
                        RedeemedPoints = batch.RedeemedPoints,
                        AwardedAtUtc = batch.AwardedAtUtc,
                        ExpiresAtUtc = batch.ExpiresAtUtc,
                        ExpiredAtUtc = batch.ExpiredAtUtc
                    });

                    continue;
                }

                existingBatch.RedeemedPoints = batch.RedeemedPoints;
                existingBatch.ExpiredAtUtc = batch.ExpiredAtUtc;
            }
        }

        private static void SyncRedemptions(LoyaltyAccountEntity existingEntity, LoyaltyAccount account)
        {
            foreach (var redemption in account.Redemptions)
            {
                if (existingEntity.Redemptions.Any(entity => entity.Id == redemption.Id))
                    continue;

                existingEntity.Redemptions.Add(new LoyaltyRedemptionEntity
                {
                    Id = redemption.Id,
                    LoyaltyAccountId = account.Id,
                    OrderId = redemption.OrderId,
                    Points = redemption.Points,
                    DiscountAmount = redemption.DiscountAmount,
                    RedeemedAtUtc = redemption.RedeemedAtUtc
                });
            }
        }
    }
}
