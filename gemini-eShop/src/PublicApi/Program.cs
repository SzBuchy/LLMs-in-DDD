using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Core.Specifications;
using Infrastructure.Data;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add DB context using SQLite (pointing to the shared Web project's database file)
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CatalogConnection") ?? "Data Source=../Web/catalog.db"));

// Enable CORS so the separate Blazor WebAssembly app can call the endpoints
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
    });
});

// Register services & repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddSingleton<IUriComposer>(new UriComposer("/"));
builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// API Endpoints

// GET /api/catalog-brands
app.MapGet("/api/catalog-brands", async (IRepository<CatalogBrand> repo) =>
{
    var brands = await repo.ListAllAsync();
    return Results.Ok(brands);
});

// GET /api/catalog-types
app.MapGet("/api/catalog-types", async (IRepository<CatalogType> repo) =>
{
    var types = await repo.ListAllAsync();
    return Results.Ok(types);
});

// GET /api/catalog-items (paged, filtered)
app.MapGet("/api/catalog-items", async (
    int? brandId, 
    int? typeId, 
    int? pageIndex, 
    int? pageSize, 
    IRepository<CatalogItem> repo,
    IUriComposer uriComposer) =>
{
    int size = pageSize ?? 10;
    int index = pageIndex ?? 0;

    var filterSpec = new CatalogFilterSpecification(brandId, typeId);
    var totalItems = await repo.CountAsync(filterSpec);

    var pagedSpec = new CatalogFilterPaginatedSpecification(index * size, size, brandId, typeId);
    var items = await repo.ListAsync(pagedSpec);

    var dtos = items.Select(i => new CatalogItemDto(
        i.Id,
        i.Name,
        i.Description,
        i.Price,
        uriComposer.ComposePicUri(i.PictureUri),
        i.CatalogTypeId,
        i.CatalogBrandId
    )).ToList();

    return Results.Ok(new { CatalogItems = dtos, TotalItems = totalItems });
});

// GET /api/catalog-items/{id}
app.MapGet("/api/catalog-items/{id:int}", async (int id, IRepository<CatalogItem> repo, IUriComposer uriComposer) =>
{
    var item = await repo.GetByIdAsync(id);
    if (item == null) return Results.NotFound();

    var dto = new CatalogItemDto(
        item.Id,
        item.Name,
        item.Description,
        item.Price,
        uriComposer.ComposePicUri(item.PictureUri),
        item.CatalogTypeId,
        item.CatalogBrandId
    );
    return Results.Ok(dto);
});

// POST /api/catalog-items (Create)
app.MapPost("/api/catalog-items", async (CreateCatalogItemRequest req, IRepository<CatalogItem> repo) =>
{
    var item = new CatalogItem
    {
        Name = req.Name,
        Description = req.Description,
        Price = req.Price,
        CatalogBrandId = req.CatalogBrandId,
        CatalogTypeId = req.CatalogTypeId,
        PictureUri = req.PictureUri ?? "http://catalogbaseurl/images/products/1.png"
    };

    await repo.AddAsync(item);
    return Results.Created($"/api/catalog-items/{item.Id}", item);
});

// PUT /api/catalog-items (Update)
app.MapPut("/api/catalog-items", async (UpdateCatalogItemRequest req, IRepository<CatalogItem> repo) =>
{
    var item = await repo.GetByIdAsync(req.Id);
    if (item == null) return Results.NotFound();

    item.UpdateDetails(req.Name, req.Description, req.Price);
    item.UpdateBrand(req.CatalogBrandId);
    item.UpdateType(req.CatalogTypeId);
    if (!string.IsNullOrEmpty(req.PictureUri))
    {
        item.UpdatePictureUri(req.PictureUri);
    }

    await repo.UpdateAsync(item);
    return Results.Ok(item);
});

// DELETE /api/catalog-items/{id}
app.MapDelete("/api/catalog-items/{id:int}", async (int id, IRepository<CatalogItem> repo) =>
{
    var item = await repo.GetByIdAsync(id);
    if (item == null) return Results.NotFound();

    await repo.DeleteAsync(item);
    return Results.NoContent();
});

app.Run();

// DTO records
public record CatalogItemDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string PictureUri,
    int CatalogTypeId,
    int CatalogBrandId
);

public record CreateCatalogItemRequest(
    string Name,
    string Description,
    decimal Price,
    string? PictureUri,
    int CatalogTypeId,
    int CatalogBrandId
);

public record UpdateCatalogItemRequest(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string? PictureUri,
    int CatalogTypeId,
    int CatalogBrandId
);
