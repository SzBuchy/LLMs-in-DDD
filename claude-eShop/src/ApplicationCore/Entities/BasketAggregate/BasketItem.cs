using ApplicationCore.Exceptions;

namespace ApplicationCore.Entities.BasketAggregate;

public class BasketItem : BaseEntity
{
    public int CatalogItemId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public BasketItem(int catalogItemId, int quantity, decimal unitPrice)
    {
        if (quantity < 1)
        {
            throw new BasketDomainException("Quantity must be at least 1");
        }

        CatalogItemId = catalogItemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void AddQuantity(int quantity) => Quantity += quantity;

    public void SetNewQuantity(int quantity)
    {
        if (quantity < 0)
        {
            throw new BasketDomainException("Quantity cannot be negative");
        }

        Quantity = quantity;
    }
}
