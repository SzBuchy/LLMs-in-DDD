using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public class CatalogContextSeed
{
    public static async Task SeedAsync(CatalogContext catalogContext, ILogger logger, int retry = 0)
    {
        var retryForAvailability = retry;
        try
        {
            if (!await catalogContext.CatalogBrands.AnyAsync())
            {
                await catalogContext.CatalogBrands.AddRangeAsync(GetPreconfiguredCatalogBrands());
                await catalogContext.SaveChangesAsync();
            }

            if (!await catalogContext.CatalogTypes.AnyAsync())
            {
                await catalogContext.CatalogTypes.AddRangeAsync(GetPreconfiguredCatalogTypes());
                await catalogContext.SaveChangesAsync();
            }

            if (!await catalogContext.CatalogItems.AnyAsync())
            {
                await catalogContext.CatalogItems.AddRangeAsync(GetPreconfiguredItems());
                await catalogContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvailability < 10)
            {
                retryForAvailability++;
                logger.LogError(ex, "An error occurred seeding the database: {Message}", ex.Message);
                await SeedAsync(catalogContext, logger, retryForAvailability);
            }
            throw;
        }
    }

    private static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
    {
        return new List<CatalogBrand>
        {
            new() { Brand = ".NET" },
            new() { Brand = "Azure" },
            new() { Brand = "SQL Server" },
            new() { Brand = "Other" },
            new() { Brand = "VS" }
        };
    }

    private static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
    {
        return new List<CatalogType>
        {
            new() { Type = "Mug" },
            new() { Type = "T-Shirt" },
            new() { Type = "Sheet" },
            new() { Type = "USB Memory Stick" }
        };
    }

    private static IEnumerable<CatalogItem> GetPreconfiguredItems()
    {
        return new List<CatalogItem>
        {
            new() { CatalogTypeId = 2, CatalogBrandId = 1, Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.50M, PictureUri = "http://catalogbaseurl/images/products/1.png" },
            new() { CatalogTypeId = 1, CatalogBrandId = 1, Description = ".NET Black Mug", Name = ".NET Black Mug", Price = 8.50M, PictureUri = "http://catalogbaseurl/images/products/2.png" },
            new() { CatalogTypeId = 2, CatalogBrandId = 4, Description = "Area Glowing T-Shirt", Name = "Area Glowing T-Shirt", Price = 12.00M, PictureUri = "http://catalogbaseurl/images/products/3.png" },
            new() { CatalogTypeId = 2, CatalogBrandId = 1, Description = ".NET Foundation T-Shirt", Name = ".NET Foundation T-Shirt", Price = 12.00M, PictureUri = "http://catalogbaseurl/images/products/4.png" },
            new() { CatalogTypeId = 3, CatalogBrandId = 1, Description = ".NET Sheet Rules", Name = ".NET Sheet Rules", Price = 8.50M, PictureUri = "http://catalogbaseurl/images/products/5.png" },
            new() { CatalogTypeId = 2, CatalogBrandId = 1, Description = "Ring Bot T-Shirt", Name = "Ring Bot T-Shirt", Price = 12.00M, PictureUri = "http://catalogbaseurl/images/products/6.png" },
            new() { CatalogTypeId = 2, CatalogBrandId = 2, Description = "Azure T-Shirt", Name = "Azure T-Shirt", Price = 12.00M, PictureUri = "http://catalogbaseurl/images/products/7.png" },
            new() { CatalogTypeId = 1, CatalogBrandId = 4, Description = "Cup & Mug Organizer", Name = "Cup & Mug Organizer", Price = 12.00M, PictureUri = "http://catalogbaseurl/images/products/8.png" },
            new() { CatalogTypeId = 3, CatalogBrandId = 4, Description = "Cup Rules Sheet", Name = "Cup Rules Sheet", Price = 8.50M, PictureUri = "http://catalogbaseurl/images/products/9.png" },
            new() { CatalogTypeId = 3, CatalogBrandId = 1, Description = ".NET Bot Sheet", Name = ".NET Bot Sheet", Price = 12.00M, PictureUri = "http://catalogbaseurl/images/products/10.png" },
            new() { CatalogTypeId = 2, CatalogBrandId = 4, Description = "Code T-Shirt", Name = "Code T-Shirt", Price = 8.50M, PictureUri = "http://catalogbaseurl/images/products/11.png" },
            new() { CatalogTypeId = 4, CatalogBrandId = 1, Description = ".NET USB Flash Drive", Name = ".NET USB Flash Drive", Price = 15.00M, PictureUri = "http://catalogbaseurl/images/products/12.png" }
        };
    }
}
