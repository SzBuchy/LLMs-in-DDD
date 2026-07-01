using ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");

        builder.OwnsOne(oi => oi.ItemOrdered, io =>
        {
            io.WithOwner();
            io.Property(p => p.ProductName).HasMaxLength(200).IsRequired();
            io.Property(p => p.PictureUri).HasMaxLength(500);
        });
    }
}
