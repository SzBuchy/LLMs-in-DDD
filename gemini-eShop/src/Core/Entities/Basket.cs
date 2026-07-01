using System.Collections.Generic;
using System.Linq;

namespace Core.Entities;

public class Basket : BaseEntity
{
    public string BuyerId { get; set; } = string.Empty;
    private readonly List<BasketItem> _items = new();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)
    {
        if (Items.All(i => i.CatalogItemId != catalogItemId))
        {
            _items.Add(new BasketItem
            {
                CatalogItemId = catalogItemId,
                Quantity = quantity,
                UnitPrice = unitPrice
            });
            return;
        }
        var existingItem = Items.First(i => i.CatalogItemId == catalogItemId);
        existingItem.AddQuantity(quantity);
    }

    public void RemoveEmptyItems()
    {
        _items.RemoveAll(i => i.Quantity == 0);
    }

    public void UpdateQuantity(int catalogItemId, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.CatalogItemId == catalogItemId);
        if (existingItem != null)
        {
            existingItem.SetQuantity(quantity);
        }
    }
}
