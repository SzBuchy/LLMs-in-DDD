using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class PickupPointTests
    {
        [Fact]
        public void Create_WhenAllValuesAreIdentical_ShouldTreatPickupPointsAsEqual()
        {
            // Arrange
            var firstPickupPoint = PickupPoint.Create("Paczkomat WAW01A", "Marszalkowska 10", "Warszawa", "00-001");
            var secondPickupPoint = PickupPoint.Create("Paczkomat WAW01A", "Marszalkowska 10", "Warszawa", "00-001");

            // Assert
            firstPickupPoint.Should().Be(secondPickupPoint);
            firstPickupPoint.GetHashCode().Should().Be(secondPickupPoint.GetHashCode());
        }

        [Fact]
        public void Create_WhenAnyValueIsDifferent_ShouldTreatPickupPointsAsDifferent()
        {
            // Arrange
            var firstPickupPoint = PickupPoint.Create("Paczkomat WAW01A", "Marszalkowska 10", "Warszawa", "00-001");
            var secondPickupPoint = PickupPoint.Create("Paczkomat WAW01B", "Marszalkowska 10", "Warszawa", "00-001");

            // Assert
            firstPickupPoint.Should().NotBe(secondPickupPoint);
        }

        [Theory]
        [InlineData("", "Marszalkowska 10", "Warszawa", "00-001")]
        [InlineData("Paczkomat WAW01A", "", "Warszawa", "00-001")]
        [InlineData("Paczkomat WAW01A", "Marszalkowska 10", "", "00-001")]
        [InlineData("Paczkomat WAW01A", "Marszalkowska 10", "Warszawa", "")]
        public void Create_WhenAnyValueIsBlank_ShouldFail(string name, string street, string city, string postalCode)
        {
            // Act
            var action = () => PickupPoint.Create(name, street, city, postalCode);

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }
    }
}
