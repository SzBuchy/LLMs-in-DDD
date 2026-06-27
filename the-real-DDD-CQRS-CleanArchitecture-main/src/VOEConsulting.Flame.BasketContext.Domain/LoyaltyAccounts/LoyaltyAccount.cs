using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts.Events;

namespace VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts
{
    public sealed class LoyaltyAccount : AggregateRoot<LoyaltyAccount>
    {
        public const int DefaultMaxPointsPerRedemption = 500;
        public const decimal DefaultPointValue = 0.01m;

        private readonly List<LoyaltyPointBatch> _pointBatches;
        private readonly List<LoyaltyRedemption> _redemptions;

        private LoyaltyAccount(
            Id<LoyaltyAccount> id,
            Id<Customer> customerId,
            int maxPointsPerRedemption,
            IEnumerable<LoyaltyPointBatch>? pointBatches = null,
            IEnumerable<LoyaltyRedemption>? redemptions = null)
            : base(id)
        {
            CustomerId = customerId.EnsureNonNull();
            MaxPointsPerRedemption = maxPointsPerRedemption.EnsurePositive();
            _pointBatches = pointBatches?.ToList() ?? [];
            _redemptions = redemptions?.ToList() ?? [];
        }

        public Id<Customer> CustomerId { get; }
        public int MaxPointsPerRedemption { get; }
        public IReadOnlyCollection<LoyaltyPointBatch> PointBatches => _pointBatches.AsReadOnly();
        public IReadOnlyCollection<LoyaltyRedemption> Redemptions => _redemptions.AsReadOnly();

        public int AvailablePoints(DateTimeOffset now) => _pointBatches.Sum(batch => batch.AvailablePoints(now));

        public int ExpiredPoints(DateTimeOffset now) => _pointBatches
            .Where(batch => batch.IsExpired(now))
            .Sum(batch => batch.Points - batch.RedeemedPoints);

        public static LoyaltyAccount Create(Id<Customer> customerId, int maxPointsPerRedemption = DefaultMaxPointsPerRedemption)
        {
            var loyaltyAccount = new LoyaltyAccount(Id<LoyaltyAccount>.New(), customerId, maxPointsPerRedemption);
            loyaltyAccount.RaiseDomainEvent(new LoyaltyAccountCreatedEvent(loyaltyAccount.Id, customerId));

            return loyaltyAccount;
        }

        public static LoyaltyAccount Restore(
            Id<LoyaltyAccount> id,
            Id<Customer> customerId,
            int maxPointsPerRedemption,
            IEnumerable<LoyaltyPointBatch> pointBatches,
            IEnumerable<LoyaltyRedemption> redemptions)
        {
            return new LoyaltyAccount(id, customerId, maxPointsPerRedemption, pointBatches, redemptions);
        }

        public void AwardPoints(Guid orderId, int points, DateTimeOffset awardedAtUtc)
        {
            orderId.EnsureNotDefault();
            points.EnsurePositive();

            if (_pointBatches.Any(batch => batch.OrderId == orderId))
                return;

            var pointBatch = LoyaltyPointBatch.Create(orderId, points, awardedAtUtc);
            _pointBatches.Add(pointBatch);

            RaiseDomainEvent(new LoyaltyPointsAwardedEvent(Id, CustomerId, orderId, points, pointBatch.ExpiresAtUtc));
        }

        public LoyaltyRedemption RedeemPoints(Guid orderId, int requestedPoints, DateTimeOffset now, decimal pointValue = DefaultPointValue)
        {
            orderId.EnsureNotDefault();
            requestedPoints.EnsurePositive();
            pointValue.EnsurePositive();
            (requestedPoints <= MaxPointsPerRedemption).EnsureTrue();

            if (_redemptions.Any(redemption => redemption.OrderId == orderId))
                return _redemptions.Single(redemption => redemption.OrderId == orderId);

            ExpirePoints(now);

            (AvailablePoints(now) >= requestedPoints).EnsureTrue();

            var pointsLeft = requestedPoints;
            foreach (var batch in _pointBatches.OrderBy(batch => batch.ExpiresAtUtc))
            {
                if (pointsLeft == 0)
                    break;

                pointsLeft -= batch.Redeem(pointsLeft, now);
            }

            var discountAmount = requestedPoints * pointValue;
            var redemption = LoyaltyRedemption.Create(orderId, requestedPoints, discountAmount, now);
            _redemptions.Add(redemption);

            RaiseDomainEvent(new LoyaltyPointsRedeemedEvent(Id, CustomerId, orderId, requestedPoints, discountAmount));

            return redemption;
        }

        public int ExpirePoints(DateTimeOffset now)
        {
            var expiredPoints = _pointBatches.Sum(batch => batch.Expire(now));
            if (expiredPoints > 0)
                RaiseDomainEvent(new LoyaltyPointsExpiredEvent(Id, CustomerId, expiredPoints));

            return expiredPoints;
        }
    }
}
