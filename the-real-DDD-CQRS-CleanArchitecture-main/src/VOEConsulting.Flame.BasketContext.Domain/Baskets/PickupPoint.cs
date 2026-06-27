namespace VOEConsulting.Flame.BasketContext.Domain.Baskets
{
    public sealed class PickupPoint : ValueObject
    {
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

        public string Name { get; }
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Street;
            yield return City;
            yield return PostalCode;
        }
    }
}
