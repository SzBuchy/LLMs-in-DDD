using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class CatalogContext : DbContext
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
    {
    }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CatalogBrand>(cb =>
        {
            cb.HasKey(x => x.Id);
            cb.Property(x => x.Brand)
              .IsRequired()
              .HasMaxLength(100);
        });

        builder.Entity<CatalogType>(ct =>
        {
            ct.HasKey(x => x.Id);
            ct.Property(x => x.Type)
              .IsRequired()
              .HasMaxLength(100);
        });

        builder.Entity<CatalogItem>(ci =>
        {
            ci.HasKey(x => x.Id);
            ci.Property(x => x.Name)
              .IsRequired()
              .HasMaxLength(100);
            ci.Property(x => x.Price)
              .HasColumnType("decimal(18,2)");
            ci.HasOne(x => x.CatalogBrand)
              .WithMany()
              .HasForeignKey(x => x.CatalogBrandId);
            ci.HasOne(x => x.CatalogType)
              .WithMany()
              .HasForeignKey(x => x.CatalogTypeId);
        });

        builder.Entity<Basket>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.BuyerId)
             .IsRequired()
             .HasMaxLength(256);
            
            var navigation = b.Metadata.FindNavigation(nameof(Basket.Items));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<BasketItem>(bi =>
        {
            bi.HasKey(x => x.Id);
            bi.Property(x => x.UnitPrice)
              .HasColumnType("decimal(18,2)");
        });

        builder.Entity<Order>(o =>
        {
            o.HasKey(x => x.Id);
            o.Property(x => x.BuyerId)
             .IsRequired()
             .HasMaxLength(256);

            o.OwnsOne(x => x.ShipToAddress, sa =>
            {
                sa.WithOwner();
                sa.Property(p => p.Street).HasMaxLength(180).IsRequired();
                sa.Property(p => p.City).HasMaxLength(100).IsRequired();
                sa.Property(p => p.State).HasMaxLength(60);
                sa.Property(p => p.Country).HasMaxLength(90).IsRequired();
                sa.Property(p => p.ZipCode).HasMaxLength(18).IsRequired();
            });

            var navigation = o.Metadata.FindNavigation(nameof(Order.OrderItems));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<OrderItem>(oi =>
        {
            oi.HasKey(x => x.Id);
            oi.Property(x => x.UnitPrice)
              .HasColumnType("decimal(18,2)");

            oi.OwnsOne(x => x.ItemOrdered, io =>
            {
                io.WithOwner();
                io.Property(p => p.ProductName).HasMaxLength(50).IsRequired();
                io.Property(p => p.PictureUri).IsRequired(false);
            });
        });
    }
}
