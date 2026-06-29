using System;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class LoyaltyTransactionDto
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public string? OrderId { get; set; }
}
