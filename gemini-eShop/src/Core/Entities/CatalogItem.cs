namespace Core.Entities;

public class CatalogItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureUri { get; set; } = string.Empty;
    public int CatalogTypeId { get; set; }
    public CatalogType? CatalogType { get; set; }
    public int CatalogBrandId { get; set; }
    public CatalogBrand? CatalogBrand { get; set; }

    public void UpdateDetails(string name, string description, decimal price)
    {
        if (price < 0) throw new ArgumentException("Price cannot be negative.");
        Name = name;
        Description = description;
        Price = price;
    }

    public void UpdateBrand(int catalogBrandId)
    {
        CatalogBrandId = catalogBrandId;
    }

    public void UpdateType(int catalogTypeId)
    {
        CatalogTypeId = catalogTypeId;
    }

    public void UpdatePictureUri(string pictureUri)
    {
        PictureUri = pictureUri;
    }
}
