using FluentAssertions;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class PickupPointTests
    {
        [Fact]
        public void Create_WhenValidArgumentsProvided_ShouldCreatePickupPoint()
        {
            // Arrange & Act
            var pickupPoint = PickupPoint.Create("Point A", "Main Street 12", "Warsaw", "00-001");

            // Assert
            pickupPoint.Should().NotBeNull();
            pickupPoint.Name.Should().Be("Point A");
            pickupPoint.Street.Should().Be("Main Street 12");
            pickupPoint.City.Should().Be("Warsaw");
            pickupPoint.PostalCode.Should().Be("00-001");
        }

        [Theory]
        [InlineData("", "Main Street 12", "Warsaw", "00-001")]
        [InlineData("Point A", "", "Warsaw", "00-001")]
        [InlineData("Point A", "Main Street 12", "", "00-001")]
        [InlineData("Point A", "Main Street 12", "Warsaw", "")]
        [InlineData(" ", "Main Street 12", "Warsaw", "00-001")]
        [InlineData(null, "Main Street 12", "Warsaw", "00-001")]
        public void Create_WhenAnyArgumentIsBlankOrNull_ShouldThrowValidationException(
            string? name, string? street, string? city, string? postalCode)
        {
            // Act
            var action = () => PickupPoint.Create(name!, street!, city!, postalCode!);

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Equals_WhenObjectsHaveIdenticalValues_ShouldBeEqual()
        {
            // Arrange
            var point1 = PickupPoint.Create("Point A", "Main Street 12", "Warsaw", "00-001");
            var point2 = PickupPoint.Create("Point A", "Main Street 12", "Warsaw", "00-001");

            // Act & Assert
            point1.Equals(point2).Should().BeTrue();
            (point1 == point2).Should().BeFalse(); // C# default class == operator check (unless overloaded, which ValueObject.cs does not overload)
            point1.GetHashCode().Should().Be(point2.GetHashCode());
        }

        [Theory]
        [InlineData("Point B", "Main Street 12", "Warsaw", "00-001")] // different name
        [InlineData("Point A", "Second Street 5", "Warsaw", "00-001")] // different street
        [InlineData("Point A", "Main Street 12", "Krakow", "00-001")] // different city
        [InlineData("Point A", "Main Street 12", "Warsaw", "00-002")] // different postal code
        public void Equals_WhenObjectsHaveDifferentValues_ShouldNotBeEqual(
            string name, string street, string city, string postalCode)
        {
            // Arrange
            var point1 = PickupPoint.Create("Point A", "Main Street 12", "Warsaw", "00-001");
            var point2 = PickupPoint.Create(name, street, city, postalCode);

            // Act & Assert
            point1.Equals(point2).Should().BeFalse();
        }
    }
}
