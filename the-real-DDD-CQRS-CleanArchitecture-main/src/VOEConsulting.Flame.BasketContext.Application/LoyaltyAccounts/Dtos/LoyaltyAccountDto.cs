namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Dtos
{
    public sealed record LoyaltyAccountDto(
        Guid Id,
        Guid CustomerId,
        int MaxPointsPerRedemption,
        int AvailablePoints,
        int ExpiredPoints,
        IReadOnlyCollection<LoyaltyPointBatchDto> PointBatches,
        IReadOnlyCollection<LoyaltyRedemptionDto> Redemptions);
}
