using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Flame.Common.Core.Exceptions;
using VOEConsulting.Infrastructure.Persistence;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Repositories
{
    public class LoyaltyAccountRepository : ILoyaltyAccountRepository
    {
        private readonly BasketAppDbContext _dbContext;
        private readonly IMapper _mapper;

        public LoyaltyAccountRepository(BasketAppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddAsync(LoyaltyAccount entity, CancellationToken cancellationToken)
        {
            var accountEntity = _mapper.Map<LoyaltyAccountEntity>(entity);
            await _dbContext.LoyaltyAccounts.AddAsync(accountEntity, cancellationToken);
        }

        public async Task<LoyaltyAccount?> GetByIdAsync(Guid id)
        {
            var accountEntity = await _dbContext.LoyaltyAccounts.FirstOrDefaultAsync(a => a.Id == id);
            return accountEntity is null ? null : _mapper.Map<LoyaltyAccount>(accountEntity);
        }

        public async Task<LoyaltyAccount?> GetByCustomerIdAsync(Guid customerId)
        {
            var accountEntity = await _dbContext.LoyaltyAccounts.FirstOrDefaultAsync(a => a.CustomerId == customerId);
            return accountEntity is null ? null : _mapper.Map<LoyaltyAccount>(accountEntity);
        }

        public async Task<bool> ExistsByCustomerIdAsync(Guid customerId)
        {
            return await _dbContext.LoyaltyAccounts.AnyAsync(a => a.CustomerId == customerId);
        }

        public async Task<IEnumerable<LoyaltyAccount>> GetAllAsync()
        {
            var accountEntities = await _dbContext.LoyaltyAccounts.ToListAsync();
            return _mapper.Map<IEnumerable<LoyaltyAccount>>(accountEntities);
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            return await _dbContext.LoyaltyAccounts.AnyAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(LoyaltyAccount entity)
        {
            Guid accountId = entity.Id;
            var accountEntity = await _dbContext.LoyaltyAccounts
                .FirstOrDefaultAsync(a => a.Id == accountId)
                ?? throw new FlameApplicationException("Loyalty account not found.");

            _mapper.Map(entity, accountEntity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var accountEntity = await _dbContext.LoyaltyAccounts.FirstOrDefaultAsync(a => a.Id == id);
            if (accountEntity is not null)
                _dbContext.LoyaltyAccounts.Remove(accountEntity);
        }
    }
}
