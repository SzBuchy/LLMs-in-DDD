using ApplicationCore.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

var useOnlyInMemoryDatabase = builder.Configuration.GetValue<bool>("UseOnlyInMemoryDatabase");

if (useOnlyInMemoryDatabase)
{
    builder.Services.AddDbContext<CatalogContext>(options => options.UseInMemoryDatabase("CatalogDb"));
    builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseInMemoryDatabase("IdentityDb"));
}
else
{
    var catalogConnection = builder.Configuration.GetConnectionString("CatalogConnection")
        ?? throw new InvalidOperationException("Connection string 'CatalogConnection' not found.");
    var identityConnection = builder.Configuration.GetConnectionString("IdentityConnection")
        ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

    builder.Services.AddDbContext<CatalogContext>(options => options.UseSqlite(catalogConnection));
    builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlite(identityConnection));
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddInfrastructureServices();
builder.Services.AddScoped<ICatalogViewModelService, CatalogViewModelService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var catalogContext = scope.ServiceProvider.GetRequiredService<CatalogContext>();
    await catalogContext.Database.EnsureCreatedAsync();
    await CatalogContextSeed.SeedAsync(catalogContext);

    var identityContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.EnsureCreatedAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await AppIdentityDbContextSeed.SeedAsync(userManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
