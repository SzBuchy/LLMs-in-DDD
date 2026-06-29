using System.Collections.Generic;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class LoyaltyAccountDto
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int Balance { get; set; }
    public decimal DiscountValue { get; set; }
    public List<LoyaltyPointsEntryDto> Entries { get; set; } = new List<LoyaltyPointsEntryDto>();
    public List<LoyaltyTransactionDto> Transactions { get; set; } = new List<LoyaltyTransactionDto>();
}
