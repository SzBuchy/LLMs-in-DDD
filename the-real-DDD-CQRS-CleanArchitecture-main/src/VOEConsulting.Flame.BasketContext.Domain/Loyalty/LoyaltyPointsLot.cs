namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty
{
    public sealed class LoyaltyPointsLot : ValueObject
    {
        private const int ExpiryYears = 1;

        private LoyaltyPointsLot(int points, DateTimeOffset earnedAtUtc)
        {
            Points = points.EnsureGreaterThan(0);
            EarnedAtUtc = earnedAtUtc;
        }

        public int Points { get; }
        public DateTimeOffset EarnedAtUtc { get; }
        public DateTimeOffset ExpiresAtUtc => EarnedAtUtc.AddYears(ExpiryYears);

        public static LoyaltyPointsLot Create(int points, DateTimeOffset earnedAtUtc) => new(points, earnedAtUtc);

        public bool IsExpired(DateTimeOffset utcNow) => utcNow >= ExpiresAtUtc;

        public LoyaltyPointsLot Reduce(int amount) => Create(Points - amount.EnsureGreaterThan(0), EarnedAtUtc);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Points;
            yield return EarnedAtUtc;
        }
    }
}
