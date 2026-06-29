using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Extensions;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty
{
    public sealed class LoyaltyPointsEntry : Entity<LoyaltyPointsEntry>
    {
        public int Amount { get; }
        public int UsedAmount { get; private set; }
        public DateTimeOffset EarnedAtUtc { get; }
        public DateTimeOffset ExpiresAtUtc { get; }

        private LoyaltyPointsEntry(int amount, DateTimeOffset earnedAtUtc)
        {
            Amount = amount.EnsurePositive();
            UsedAmount = 0;
            EarnedAtUtc = earnedAtUtc;
            ExpiresAtUtc = earnedAtUtc.AddYears(1);
        }

        private LoyaltyPointsEntry(Id<LoyaltyPointsEntry> id, int amount, int usedAmount, DateTimeOffset earnedAtUtc, DateTimeOffset expiresAtUtc)
            : base(id)
        {
            Amount = amount.EnsurePositive();
            UsedAmount = usedAmount.EnsureNonNegative();
            EarnedAtUtc = earnedAtUtc;
            ExpiresAtUtc = expiresAtUtc;
        }

        public static LoyaltyPointsEntry Create(int amount, DateTimeOffset earnedAtUtc)
        {
            return new LoyaltyPointsEntry(amount, earnedAtUtc);
        }

        public static LoyaltyPointsEntry Reconstitute(Id<LoyaltyPointsEntry> id, int amount, int usedAmount, DateTimeOffset earnedAtUtc, DateTimeOffset expiresAtUtc)
        {
            return new LoyaltyPointsEntry(id, amount, usedAmount, earnedAtUtc, expiresAtUtc);
        }

        public int GetAvailablePoints(DateTimeOffset nowUtc)
        {
            if (nowUtc >= ExpiresAtUtc)
            {
                return 0; // Expired
            }
            return Amount - UsedAmount;
        }

        public void UsePoints(int pointsToUse)
        {
            pointsToUse.EnsurePositive();
            var available = Amount - UsedAmount;
            if (pointsToUse > available)
            {
                throw new ValidationException("Cannot use more points than available in this entry.");
            }
            UsedAmount += pointsToUse;
        }
    }
}
