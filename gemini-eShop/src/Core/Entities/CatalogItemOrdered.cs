namespace Core.Entities;

public record CatalogItemOrdered(
    int CatalogItemId,
    string ProductName,
    string PictureUri
);
