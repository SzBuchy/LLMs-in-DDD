namespace Web.ViewModels;

public class OrderItemViewModel
{
    public int CatalogItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
    public string PictureUri { get; set; } = string.Empty;
}
