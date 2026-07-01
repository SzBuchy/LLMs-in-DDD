namespace Core.Entities;

public class OrderItem : BaseEntity
{
    public CatalogItemOrdered ItemOrdered { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
}
