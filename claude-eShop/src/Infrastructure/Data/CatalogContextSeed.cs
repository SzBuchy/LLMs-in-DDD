using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class CatalogContextSeed
{
    public static async Task SeedAsync(CatalogContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.CatalogBrands.AnyAsync())
        {
            return;
        }

        var brands = new List<CatalogBrand>
        {
            new(".NET"),
            new("Visual Studio"),
            new("SQL Server"),
            new("Other"),
        };
        await context.CatalogBrands.AddRangeAsync(brands);

        var types = new List<CatalogType>
        {
            new("Mug"),
            new("T-Shirt"),
            new("Sheet"),
            new("USB Memory Stick"),
        };
        await context.CatalogTypes.AddRangeAsync(types);

        await context.SaveChangesAsync();

        var items = new List<CatalogItem>
        {
            new(types[0].Id, brands[0].Id, ".NET Bot Black Mug", "Coffee mug with the .NET bot", 8.50m, "images/products/1.png"),
            new(types[1].Id, brands[0].Id, ".NET Black & White T-Shirt", "T-Shirt with the .NET logo", 12.00m, "images/products/2.png"),
            new(types[1].Id, brands[2].Id, "Prism White T-Shirt", "SQL Server branded prism t-shirt", 12.00m, "images/products/3.png"),
            new(types[3].Id, brands[1].Id, "Visual Studio USB Stick", "8GB USB memory stick", 8.50m, "images/products/4.png"),
            new(types[0].Id, brands[1].Id, "Visual Studio Mug", "Coffee mug with the Visual Studio logo", 8.50m, "images/products/5.png"),
            new(types[2].Id, brands[3].Id, "Roslyn Red Sheet", "Roslyn compiler themed sheet", 8.50m, "images/products/6.png"),
        };
        await context.CatalogItems.AddRangeAsync(items);

        await context.SaveChangesAsync();
    }
}
