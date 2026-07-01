using System;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Web.Interfaces;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContexts using SQLite for ease of local configuration
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CatalogConnection") ?? "Data Source=catalog.db"));

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection") ?? "Data Source=identity.db"));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Register services & repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ICatalogViewModelService, CatalogViewModelService>();
builder.Services.AddScoped<IBasketViewModelService, BasketViewModelService>();
builder.Services.AddSingleton<IUriComposer>(new UriComposer("/"));
builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

builder.Services.AddRazorPages();

var app = builder.Build();

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Startup");
    try
    {
        var catalogContext = services.GetRequiredService<CatalogContext>();
        await catalogContext.Database.EnsureCreatedAsync();
        await CatalogContextSeed.SeedAsync(catalogContext, logger);

        var identityContext = services.GetRequiredService<AppIdentityDbContext>();
        await identityContext.Database.EnsureCreatedAsync();
        
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await AppIdentityDbContextSeed.SeedAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles("/admin");
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapFallbackToFile("admin/{*path:nonfile}", "admin/index.html");

app.Run();
