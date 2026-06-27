namespace VOEConsulting.Flame.BasketContext.Domain.Baskets
{
    public sealed class PickupPoint : ValueObject
    {
        private PickupPoint(string pointName, string street, string city, string postalCode)
        {
            PointName = pointName.EnsureNonBlank();
            Street = street.EnsureNonBlank();
            City = city.EnsureNonBlank();
            PostalCode = postalCode.EnsureNonBlank();
        }

        public static PickupPoint Create(string pointName, string street, string city, string postalCode)
        {
            return new PickupPoint(pointName, street, city, postalCode);
        }

        public string PointName { get; }
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PointName;
            yield return Street;
            yield return City;
            yield return PostalCode;
        }
    }
}
