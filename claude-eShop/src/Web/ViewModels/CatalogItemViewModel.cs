namespace Web.ViewModels;

public class CatalogItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureUri { get; set; } = string.Empty;
    public int? CatalogTypeId { get; set; }
    public string? CatalogTypeName { get; set; }
    public int? CatalogBrandId { get; set; }
    public string? CatalogBrandName { get; set; }
}
