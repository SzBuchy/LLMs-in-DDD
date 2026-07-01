namespace ApplicationCore.Entities.OrderAggregate;

// Snapshot of the catalog item as it was at the time the order was placed, so that later
// changes to price/name/picture in the catalog never rewrite historical orders.
public class CatalogItemOrdered : ValueObject
{
    public int CatalogItemId { get; private set; }
    public string ProductName { get; private set; }
    public string PictureUri { get; private set; }

    private CatalogItemOrdered()
    {
        ProductName = string.Empty;
        PictureUri = string.Empty;
    }

    public CatalogItemOrdered(int catalogItemId, string productName, string pictureUri)
    {
        CatalogItemId = catalogItemId;
        ProductName = productName;
        PictureUri = pictureUri;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CatalogItemId;
        yield return ProductName;
        yield return PictureUri;
    }
}
