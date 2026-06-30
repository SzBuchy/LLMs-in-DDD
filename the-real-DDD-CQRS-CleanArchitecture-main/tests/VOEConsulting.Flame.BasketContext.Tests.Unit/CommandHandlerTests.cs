using NSubstitute;
using FluentAssertions;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.UpdateBasketItem;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.DeleteBasketItem;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.ClearBasket;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.CalculateTotalAmount;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Baskets.Services;
using VOEConsulting.Flame.BasketContext.Tests.Unit.Factories;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Events;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class CommandHandlerTests
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly ICouponService _couponService;

        public CommandHandlerTests()
        {
            _basketRepository = Substitute.For<IBasketRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            _couponService = Substitute.For<ICouponService>();
        }

        [Fact]
        public async Task UpdateBasketItemCountCommandHandler_ShouldUpdateQuantity_AndCallUpdateAsync()
        {
            // Arrange
            var basket = BasketFactory.Create();
            var seller = SellerFactory.Create();
            var quantity = QuantityFactory.Create(value: 2, limit: 10);
            var item = BasketItemFactory.Create(quantity: quantity, seller: seller);
            basket.AddItem(item);
            basket.ClearEvents();

            _basketRepository.GetByIdAsync(basket.Id).Returns(basket);

            var handler = new UpdateBasketItemCountCommandHandler(_basketRepository, _unitOfWork, _domainEventDispatcher);
            var command = new UpdateBasketItemCountCommand(basket.Id, item.Id, 5);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(item.Id);
            item.Quantity.Value.Should().Be(5);
            await _basketRepository.Received(1).UpdateAsync(basket);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteBasketItemCommandHandler_ShouldRemoveItem_AndCallUpdateAsync()
        {
            // Arrange
            var basket = BasketFactory.Create();
            var seller = SellerFactory.Create();
            var item = BasketItemFactory.Create(seller: seller);
            basket.AddItem(item);
            basket.ClearEvents();

            _basketRepository.GetByIdAsync(basket.Id).Returns(basket);

            var handler = new DeleteBasketItemCommandHandler(_basketRepository, _unitOfWork, _domainEventDispatcher);
            var command = new DeleteBasketItemCommand(basket.Id, item.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(item.Id);
            basket.BasketItems.SelectMany(x => x.Value.Items).Should().NotContain(item);
            await _basketRepository.Received(1).UpdateAsync(basket);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ClearBasketCommandHandler_ShouldRemoveAllItems_AndCallUpdateAsync()
        {
            // Arrange
            var basket = BasketFactory.Create();
            var seller = SellerFactory.Create();
            var item = BasketItemFactory.Create(seller: seller);
            basket.AddItem(item);
            basket.ClearEvents();

            _basketRepository.GetByIdAsync(basket.Id).Returns(basket);

            var handler = new ClearBasketCommandHandler(_basketRepository, _unitOfWork, _domainEventDispatcher);
            var command = new ClearBasketCommand(basket.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(basket.Id);
            basket.BasketItems.Should().BeEmpty();
            await _basketRepository.Received(1).UpdateAsync(basket);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CalculateTotalAmountCommandHandler_ShouldPerformCalculations_AndCallUpdateAsync()
        {
            // Arrange
            var basket = BasketFactory.Create(taxPercentage: 10);
            var seller = SellerFactory.Create(shippingLimit: 100, shippingCost: 10);
            var quantity = QuantityFactory.Create(value: 2, limit: 10, pricePerUnit: 20); // total 40
            var item = BasketItemFactory.Create(quantity: quantity, seller: seller);
            basket.AddItem(item);
            basket.ClearEvents();

            _basketRepository.GetByIdAsync(basket.Id).Returns(basket);

            var handler = new CalculateTotalAmountCommandHandler(_basketRepository, _couponService, _unitOfWork, _domainEventDispatcher);
            var command = new CalculateTotalAmountCommand(basket.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            // Total amount = (40 + 10 shipping) * 1.1 tax = 55
            result.Value.Should().Be(55m);
            basket.TotalAmount.Should().Be(55m);
            await _basketRepository.Received(1).UpdateAsync(basket);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
