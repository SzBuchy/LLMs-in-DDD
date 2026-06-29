using VOEConsulting.Flame.Common.Domain.Extensions;

namespace VOEConsulting.Flame.Common.Domain
{
    public sealed class PickupPoint : ValueObject
    {
        public string Name { get; }
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }

        private PickupPoint(string name, string street, string city, string postalCode)
        {
            Name = name.EnsureNonBlank();
            Street = street.EnsureNonBlank();
            City = city.EnsureNonBlank();
            PostalCode = postalCode.EnsureNonBlank();
        }

        public static PickupPoint Create(string name, string street, string city, string postalCode)
        {
            return new PickupPoint(name, street, city, postalCode);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Street;
            yield return City;
            yield return PostalCode;
        }
    }
}
