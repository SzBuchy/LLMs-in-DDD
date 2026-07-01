namespace Web.ViewModels;

public class BasketItemViewModel
{
    public int Id { get; set; }
    public int CatalogItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PictureUri { get; set; } = string.Empty;
}
