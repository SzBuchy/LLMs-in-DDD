using System.Collections.Generic;
using System.Linq;

namespace Web.ViewModels;

public class BasketViewModel
{
    public int Id { get; set; }
    public string BuyerId { get; set; } = string.Empty;
    public List<BasketItemViewModel> Items { get; set; } = new();

    public decimal Total()
    {
        return Items.Sum(x => x.UnitPrice * x.Quantity);
    }
}
