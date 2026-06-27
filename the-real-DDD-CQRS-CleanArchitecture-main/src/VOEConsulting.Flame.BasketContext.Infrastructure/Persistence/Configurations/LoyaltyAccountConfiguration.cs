using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccountEntity>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccountEntity> builder)
    {
        builder.ToTable("LoyaltyAccounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CustomerId)
            .IsRequired();

        builder.HasIndex(a => a.CustomerId)
            .IsUnique();

        builder.OwnsMany(a => a.PointsLots, lots =>
        {
            lots.ToTable("LoyaltyPointsLots");
            lots.WithOwner().HasForeignKey("LoyaltyAccountId");

            lots.Property(l => l.Points)
                .IsRequired();

            lots.Property(l => l.EarnedAtUtc)
                .IsRequired();
        });
    }
}
