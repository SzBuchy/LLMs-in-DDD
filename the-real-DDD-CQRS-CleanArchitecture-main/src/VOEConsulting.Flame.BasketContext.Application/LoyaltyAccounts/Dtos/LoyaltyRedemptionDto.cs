namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Dtos
{
    public sealed record LoyaltyRedemptionDto(
        Guid Id,
        Guid OrderId,
        int Points,
        decimal DiscountAmount,
        DateTimeOffset RedeemedAtUtc);
}
