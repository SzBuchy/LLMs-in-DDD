namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Dtos
{
    public sealed record LoyaltyPointBatchDto(
        Guid Id,
        Guid OrderId,
        int Points,
        int RedeemedPoints,
        int AvailablePoints,
        DateTimeOffset AwardedAtUtc,
        DateTimeOffset ExpiresAtUtc,
        DateTimeOffset? ExpiredAtUtc);
}
