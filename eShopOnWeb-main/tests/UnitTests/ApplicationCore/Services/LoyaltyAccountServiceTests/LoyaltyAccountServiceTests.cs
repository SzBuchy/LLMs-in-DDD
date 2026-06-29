using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.LoyaltyAccountServiceTests;

public class LoyaltyAccountServiceTests
{
    private readonly ILoyaltyAccountRepository _mockLoyaltyAccountRepository = Substitute.For<ILoyaltyAccountRepository>();
    private readonly string _customerId = "customer-999";

    [Fact]
    public async Task GetOrCreateAccountCreatesNewAccountIfNoneExists()
    {
        _mockLoyaltyAccountRepository.GetByCustomerIdAsync(_customerId, Arg.Any<CancellationToken>())
            .Returns((LoyaltyAccount?)null);

        var service = new LoyaltyAccountService(_mockLoyaltyAccountRepository);

        var result = await service.GetOrCreateAccountAsync(_customerId);

        Assert.NotNull(result);
        Assert.Equal(_customerId, result.CustomerId);
        await _mockLoyaltyAccountRepository.Received(1).AddAsync(Arg.Is<LoyaltyAccount>(la => la.CustomerId == _customerId), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrCreateAccountReturnsExistingAccount()
    {
        var existingAccount = new LoyaltyAccount(_customerId);
        _mockLoyaltyAccountRepository.GetByCustomerIdAsync(_customerId, Arg.Any<CancellationToken>())
            .Returns(existingAccount);

        var service = new LoyaltyAccountService(_mockLoyaltyAccountRepository);

        var result = await service.GetOrCreateAccountAsync(_customerId);

        Assert.NotNull(result);
        Assert.Equal(existingAccount, result);
        await _mockLoyaltyAccountRepository.DidNotReceive().AddAsync(Arg.Any<LoyaltyAccount>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AwardPointsAddsPointsSuccessfully()
    {
        var existingAccount = new LoyaltyAccount(_customerId);
        _mockLoyaltyAccountRepository.GetByCustomerIdAsync(_customerId, Arg.Any<CancellationToken>())
            .Returns(existingAccount);

        var service = new LoyaltyAccountService(_mockLoyaltyAccountRepository);

        var result = await service.AwardPointsAsync(_customerId, 100, "order-001");

        Assert.Equal(100, result.GetActivePointsBalance(DateTimeOffset.Now));
        await _mockLoyaltyAccountRepository.Received(1).UpdateAsync(existingAccount, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SpendPointsSubtractsPointsSuccessfully()
    {
        var existingAccount = new LoyaltyAccount(_customerId);
        existingAccount.AwardPoints(200, DateTimeOffset.Now);
        _mockLoyaltyAccountRepository.GetByCustomerIdAsync(_customerId, Arg.Any<CancellationToken>())
            .Returns(existingAccount);

        var service = new LoyaltyAccountService(_mockLoyaltyAccountRepository);

        var result = await service.SpendPointsAsync(_customerId, 150, "order-002");

        Assert.Equal(50, result.GetActivePointsBalance(DateTimeOffset.Now));
        await _mockLoyaltyAccountRepository.Received(1).UpdateAsync(existingAccount, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBalanceReturnsZeroIfNoAccountExists()
    {
        _mockLoyaltyAccountRepository.GetByCustomerIdAsync(_customerId, Arg.Any<CancellationToken>())
            .Returns((LoyaltyAccount?)null);

        var service = new LoyaltyAccountService(_mockLoyaltyAccountRepository);

        var balance = await service.GetBalanceAsync(_customerId);

        Assert.Equal(0, balance);
    }
}
