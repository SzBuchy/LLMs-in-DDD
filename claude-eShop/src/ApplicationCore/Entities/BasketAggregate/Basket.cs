using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities.BasketAggregate;

// Aggregate root. All modifications to basket items must go through this type so invariants
// (no negative/zero quantities left lying around, one line per catalog item) are always upheld.
public class Basket : BaseEntity, IAggregateRoot
{
    public string BuyerId { get; private set; }

    private readonly List<BasketItem> _items = new();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public Basket(string buyerId)
    {
        BuyerId = !string.IsNullOrWhiteSpace(buyerId) ? buyerId : throw new BasketDomainException("BuyerId is required");
    }

    public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)
    {
        if (quantity < 1)
        {
            throw new BasketDomainException("Quantity must be at least 1");
        }

        var existingItem = _items.FirstOrDefault(i => i.CatalogItemId == catalogItemId);
        if (existingItem == null)
        {
            _items.Add(new BasketItem(catalogItemId, quantity, unitPrice));
            return;
        }

        existingItem.AddQuantity(quantity);
    }

    public void RemoveEmptyItems() => _items.RemoveAll(i => i.Quantity == 0);

    public void SetNewBuyerId(string buyerId)
    {
        BuyerId = !string.IsNullOrWhiteSpace(buyerId) ? buyerId : throw new BasketDomainException("BuyerId is required");
    }

    public void SetQuantities(IDictionary<int, int> quantities)
    {
        foreach (var item in _items)
        {
            if (quantities.TryGetValue(item.Id, out var quantity))
            {
                item.SetNewQuantity(quantity);
            }
        }

        RemoveEmptyItems();
    }
}
