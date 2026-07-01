using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities;

public class CatalogItem : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string PictureUri { get; private set; }

    public int CatalogTypeId { get; private set; }
    public CatalogType? CatalogType { get; private set; }

    public int CatalogBrandId { get; private set; }
    public CatalogBrand? CatalogBrand { get; private set; }

    public CatalogItem(
        int catalogTypeId,
        int catalogBrandId,
        string name,
        string description,
        decimal price,
        string pictureUri)
    {
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
        Name = name;
        Description = description;
        Price = price;
        PictureUri = pictureUri;
    }

    public void UpdateDetails(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required", nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
        }

        Name = name;
        Description = description;
        Price = price;
    }

    public void UpdateBrand(int catalogBrandId) => CatalogBrandId = catalogBrandId;

    public void UpdateType(int catalogTypeId) => CatalogTypeId = catalogTypeId;

    public void UpdatePictureUri(string pictureUri) => PictureUri = pictureUri;
}
