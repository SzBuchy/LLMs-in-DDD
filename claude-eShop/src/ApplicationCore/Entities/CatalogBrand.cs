using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities;

public class CatalogBrand : BaseEntity, IAggregateRoot
{
    public string Brand { get; private set; }

    public CatalogBrand(string brand)
    {
        Brand = !string.IsNullOrWhiteSpace(brand) ? brand : throw new ArgumentException("Brand is required", nameof(brand));
    }

    public void UpdateBrand(string brand)
    {
        Brand = !string.IsNullOrWhiteSpace(brand) ? brand : throw new ArgumentException("Brand is required", nameof(brand));
    }
}
