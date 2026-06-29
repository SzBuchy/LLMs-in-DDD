using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class LoyaltyAccountService : ILoyaltyAccountService
{
    private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;

    public LoyaltyAccountService(ILoyaltyAccountRepository loyaltyAccountRepository)
    {
        _loyaltyAccountRepository = loyaltyAccountRepository;
    }

    public async Task<LoyaltyAccount> GetOrCreateAccountAsync(string customerId, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));

        var account = await _loyaltyAccountRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        if (account == null)
        {
            account = new LoyaltyAccount(customerId);
            await _loyaltyAccountRepository.AddAsync(account, cancellationToken);
        }

        return account;
    }

    public async Task<LoyaltyAccount> AwardPointsAsync(string customerId, int points, string? orderId = null, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));
        Guard.Against.OutOfRange(points, nameof(points), 1, int.MaxValue);

        var account = await GetOrCreateAccountAsync(customerId, cancellationToken);
        account.AwardPoints(points, DateTimeOffset.Now, orderId);

        await _loyaltyAccountRepository.UpdateAsync(account, cancellationToken);
        return account;
    }

    public async Task<LoyaltyAccount> SpendPointsAsync(string customerId, int points, string? orderId = null, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));
        Guard.Against.OutOfRange(points, nameof(points), 1, LoyaltyAccount.MaxPointsPerUsage);

        var account = await _loyaltyAccountRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Konto lojalnościowe klienta nie istnieje.");
        }

        account.SpendPoints(points, DateTimeOffset.Now, orderId);

        await _loyaltyAccountRepository.UpdateAsync(account, cancellationToken);
        return account;
    }

    public async Task<int> GetBalanceAsync(string customerId, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));

        var account = await _loyaltyAccountRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        if (account == null)
        {
            return 0;
        }

        return account.GetActivePointsBalance(DateTimeOffset.Now);
    }
}
