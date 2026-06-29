using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class LoyaltyAccountRepository : EfRepository<LoyaltyAccount>, ILoyaltyAccountRepository
{
    private readonly CatalogContext _dbContext;

    public LoyaltyAccountRepository(CatalogContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoyaltyAccount?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<LoyaltyAccount>()
            .Include(la => la.Entries)
            .Include(la => la.Transactions)
            .FirstOrDefaultAsync(la => la.CustomerId == customerId, cancellationToken);
    }
}
