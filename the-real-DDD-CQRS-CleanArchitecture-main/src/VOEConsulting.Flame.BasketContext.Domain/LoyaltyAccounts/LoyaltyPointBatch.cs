namespace VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts
{
    public sealed class LoyaltyPointBatch : Entity<LoyaltyPointBatch>
    {
        private LoyaltyPointBatch(
            Id<LoyaltyPointBatch> id,
            Guid orderId,
            int points,
            int redeemedPoints,
            DateTimeOffset awardedAtUtc,
            DateTimeOffset expiresAtUtc,
            DateTimeOffset? expiredAtUtc)
            : base(id)
        {
            OrderId = orderId.EnsureNotDefault();
            Points = points.EnsurePositive();
            RedeemedPoints = redeemedPoints.EnsureNonNegative();
            (redeemedPoints <= points).EnsureTrue();
            AwardedAtUtc = awardedAtUtc;
            ExpiresAtUtc = expiresAtUtc;
            ExpiredAtUtc = expiredAtUtc;
        }

        public Guid OrderId { get; }
        public int Points { get; }
        public int RedeemedPoints { get; private set; }
        public DateTimeOffset AwardedAtUtc { get; }
        public DateTimeOffset ExpiresAtUtc { get; }
        public DateTimeOffset? ExpiredAtUtc { get; private set; }

        public int AvailablePoints(DateTimeOffset now)
        {
            if (ExpiredAtUtc is not null || ExpiresAtUtc <= now)
                return 0;

            return Points - RedeemedPoints;
        }

        public bool IsExpired(DateTimeOffset now) => ExpiredAtUtc is not null || ExpiresAtUtc <= now;

        public static LoyaltyPointBatch Create(Guid orderId, int points, DateTimeOffset awardedAtUtc)
        {
            return new LoyaltyPointBatch(
                Id<LoyaltyPointBatch>.New(),
                orderId,
                points,
                0,
                awardedAtUtc,
                awardedAtUtc.AddYears(1),
                null);
        }

        public static LoyaltyPointBatch Restore(
            Id<LoyaltyPointBatch> id,
            Guid orderId,
            int points,
            int redeemedPoints,
            DateTimeOffset awardedAtUtc,
            DateTimeOffset expiresAtUtc,
            DateTimeOffset? expiredAtUtc)
        {
            return new LoyaltyPointBatch(id, orderId, points, redeemedPoints, awardedAtUtc, expiresAtUtc, expiredAtUtc);
        }

        public int Redeem(int requestedPoints, DateTimeOffset now)
        {
            requestedPoints.EnsurePositive();

            var pointsToRedeem = Math.Min(requestedPoints, AvailablePoints(now));
            RedeemedPoints += pointsToRedeem;

            return pointsToRedeem;
        }

        public int Expire(DateTimeOffset now)
        {
            if (ExpiredAtUtc is not null || ExpiresAtUtc > now)
                return 0;

            var expiredPoints = Points - RedeemedPoints;
            ExpiredAtUtc = now;

            return expiredPoints;
        }
    }
}
