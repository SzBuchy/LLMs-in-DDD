using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.CreateLoyaltyAccount;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.AddPoints;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemPoints;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Queries.GetLoyaltyAccount;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Services;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Events;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using Xunit;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class LoyaltyCommandHandlerTests
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IMapper _mapper;

        public LoyaltyCommandHandlerTests()
        {
            _loyaltyAccountRepository = Substitute.For<ILoyaltyAccountRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            _mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task CreateAccount_WhenNewCustomer_ShouldCreateSaveAndDispatchEvents()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new CreateLoyaltyAccountCommand(customerId);

            _loyaltyAccountRepository.GetByCustomerIdAsync(Arg.Any<Id<Customer>>(), Arg.Any<CancellationToken>())
                .Returns((LoyaltyAccount?)null);

            var handler = new CreateLoyaltyAccountCommandHandler(_loyaltyAccountRepository, _unitOfWork, _domainEventDispatcher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            await _loyaltyAccountRepository.Received(1).AddAsync(Arg.Any<LoyaltyAccount>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _domainEventDispatcher.Received(1).DispatchAsync(
                Arg.Is<IEnumerable<IDomainEvent>>(events => events.Any(e => e is LoyaltyAccountCreatedEvent)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateAccount_WhenExistingCustomer_ShouldReturnConflictError()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new CreateLoyaltyAccountCommand(customerId);
            var existingAccount = LoyaltyAccount.Create(customerId);

            _loyaltyAccountRepository.GetByCustomerIdAsync(Arg.Any<Id<Customer>>(), Arg.Any<CancellationToken>())
                .Returns(existingAccount);

            var handler = new CreateLoyaltyAccountCommandHandler(_loyaltyAccountRepository, _unitOfWork, _domainEventDispatcher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.ErrorType.Should().Be(Common.Domain.Errors.ErrorType.Conflict);
        }

        [Fact]
        public async Task AddPoints_WhenAccountExists_ShouldAddSaveAndDispatchEvents()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new AddPointsCommand(customerId, 100);
            var account = LoyaltyAccount.Create(customerId);

            _loyaltyAccountRepository.GetByCustomerIdAsync(Arg.Any<Id<Customer>>(), Arg.Any<CancellationToken>())
                .Returns(account);

            var handler = new AddPointsCommandHandler(_loyaltyAccountRepository, _unitOfWork, _domainEventDispatcher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            account.GetAvailablePointsBalance(DateTimeOffset.UtcNow).Should().Be(100);

            await _loyaltyAccountRepository.Received(1).UpdateAsync(account, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _domainEventDispatcher.Received(1).DispatchAsync(
                Arg.Is<IEnumerable<IDomainEvent>>(events => events.Any(e => e is PointsAddedEvent)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RedeemPoints_WhenSufficientPoints_ShouldRedeemSaveAndDispatchEvents()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new RedeemPointsCommand(customerId, 50);
            var account = LoyaltyAccount.Create(customerId);
            account.AddPoints(100, DateTimeOffset.UtcNow);
            account.ClearEvents(); // Clear events from creation/adding

            _loyaltyAccountRepository.GetByCustomerIdAsync(Arg.Any<Id<Customer>>(), Arg.Any<CancellationToken>())
                .Returns(account);

            var handler = new RedeemPointsCommandHandler(_loyaltyAccountRepository, _unitOfWork, _domainEventDispatcher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            account.GetAvailablePointsBalance(DateTimeOffset.UtcNow).Should().Be(50);

            await _loyaltyAccountRepository.Received(1).UpdateAsync(account, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _domainEventDispatcher.Received(1).DispatchAsync(
                Arg.Is<IEnumerable<IDomainEvent>>(events => events.Any(e => e is PointsRedeemedEvent)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RedeemPoints_WhenInsufficientPoints_ShouldReturnValidationError()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new RedeemPointsCommand(customerId, 150);
            var account = LoyaltyAccount.Create(customerId);
            account.AddPoints(100, DateTimeOffset.UtcNow);

            _loyaltyAccountRepository.GetByCustomerIdAsync(Arg.Any<Id<Customer>>(), Arg.Any<CancellationToken>())
                .Returns(account);

            var handler = new RedeemPointsCommandHandler(_loyaltyAccountRepository, _unitOfWork, _domainEventDispatcher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.ErrorType.Should().Be(Common.Domain.Errors.ErrorType.Validation);
        }

        [Fact]
        public async Task GetLoyaltyAccount_WhenAccountExists_ShouldReturnMappedDto()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var query = new GetLoyaltyAccountQuery(customerId);
            var account = LoyaltyAccount.Create(customerId);
            var expectedDto = new LoyaltyAccountDto { CustomerId = customerId, AvailablePointsBalance = 0 };

            _loyaltyAccountRepository.GetByCustomerIdAsync(Arg.Any<Id<Customer>>(), Arg.Any<CancellationToken>())
                .Returns(account);
            _mapper.Map<LoyaltyAccountDto>(account).Returns(expectedDto);

            var handler = new GetLoyaltyAccountQueryHandler(_loyaltyAccountRepository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(expectedDto);
        }
    }
}
