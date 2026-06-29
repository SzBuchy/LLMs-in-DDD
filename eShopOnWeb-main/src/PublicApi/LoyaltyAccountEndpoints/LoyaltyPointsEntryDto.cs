using System;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class LoyaltyPointsEntryDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int SpentQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string? OrderId { get; set; }
    public bool IsExpired { get; set; }
}
