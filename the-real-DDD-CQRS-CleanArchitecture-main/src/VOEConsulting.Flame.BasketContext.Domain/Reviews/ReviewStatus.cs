using Ardalis.SmartEnum;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews
{
    public sealed class ReviewStatus : SmartEnum<ReviewStatus>
    {
        public static readonly ReviewStatus PendingModeration = new(nameof(PendingModeration), 1);
        public static readonly ReviewStatus Published = new(nameof(Published), 2);
        public static readonly ReviewStatus Rejected = new(nameof(Rejected), 3);
        public static readonly ReviewStatus Withdrawn = new(nameof(Withdrawn), 4);

        private ReviewStatus(string name, int value) : base(name, value) { }
    }
}
