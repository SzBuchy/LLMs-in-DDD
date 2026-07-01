namespace Core.Entities;

public class BasketItem : BaseEntity
{
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int CatalogItemId { get; set; }
    public int BasketId { get; set; }

    public void AddQuantity(int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantity to add cannot be negative.");
        Quantity += quantity;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantity cannot be negative.");
        Quantity = quantity;
    }
}
