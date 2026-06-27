using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class LoyaltyAccountRepository : ILoyaltyAccountRepository
{
    private readonly IRepository<LoyaltyAccount> _repository;

    public LoyaltyAccountRepository(IRepository<LoyaltyAccount> repository)
    {
        _repository = repository;
    }

    public Task<LoyaltyAccount> AddAsync(LoyaltyAccount account) => _repository.AddAsync(account);

    public Task UpdateAsync(LoyaltyAccount account) => _repository.UpdateAsync(account);

    public Task<LoyaltyAccount?> GetByBuyerIdAsync(string buyerId) =>
        _repository.FirstOrDefaultAsync(new LoyaltyAccountByBuyerIdSpecification(buyerId));
}
