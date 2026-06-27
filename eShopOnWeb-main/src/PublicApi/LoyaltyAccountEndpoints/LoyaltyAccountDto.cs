using System;
using System.Collections.Generic;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class LoyaltyAccountDto
{
    public int Id { get; set; }
    public string BuyerId { get; set; } = string.Empty;
    public int AvailablePoints { get; set; }
    public int MaxPointsPerRedemption { get; set; }
    public decimal DiscountAmountPerPoint { get; set; }
    public List<LoyaltyPointGrantDto> PointGrants { get; set; } = new();
    public List<LoyaltyPointRedemptionDto> Redemptions { get; set; } = new();
}

public class LoyaltyPointGrantDto
{
    public int Id { get; set; }
    public int? SourceOrderId { get; set; }
    public int PointsAwarded { get; set; }
    public int PointsRemaining { get; set; }
    public DateTimeOffset AwardedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}

public class LoyaltyPointRedemptionDto
{
    public int Id { get; set; }
    public int PointsRedeemed { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTimeOffset RedeemedAt { get; set; }
}
