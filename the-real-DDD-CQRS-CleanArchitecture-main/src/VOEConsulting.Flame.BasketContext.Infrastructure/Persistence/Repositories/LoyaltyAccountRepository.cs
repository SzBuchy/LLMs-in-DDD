using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Services;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Infrastructure.Persistence;
using VOEConsulting.Flame.Common.Domain;

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

        public async Task AddAsync(LoyaltyAccount account, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<LoyaltyAccountEntity>(account);
            await _dbContext.LoyaltyAccounts.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(LoyaltyAccount account, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<LoyaltyAccountEntity>(account);
            _dbContext.LoyaltyAccounts.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<LoyaltyAccount?> GetByIdAsync(Id<LoyaltyAccount> id, CancellationToken cancellationToken = default)
        {
            var guid = id.Value;
            var entity = await _dbContext.LoyaltyAccounts
                .Include(la => la.PointsEntries)
                .FirstOrDefaultAsync(la => la.Id == guid, cancellationToken);

            return entity != null ? _mapper.Map<LoyaltyAccount>(entity) : null;
        }

        public async Task<LoyaltyAccount?> GetByCustomerIdAsync(Id<Customer> customerId, CancellationToken cancellationToken = default)
        {
            var guid = customerId.Value;
            var entity = await _dbContext.LoyaltyAccounts
                .Include(la => la.PointsEntries)
                .FirstOrDefaultAsync(la => la.CustomerId == guid, cancellationToken);

            return entity != null ? _mapper.Map<LoyaltyAccount>(entity) : null;
        }
    }
}
