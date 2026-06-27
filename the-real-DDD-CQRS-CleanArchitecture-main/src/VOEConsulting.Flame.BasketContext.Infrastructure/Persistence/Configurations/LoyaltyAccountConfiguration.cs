using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Configurations
{
    public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccountEntity>
    {
        public void Configure(EntityTypeBuilder<LoyaltyAccountEntity> builder)
        {
            builder.ToTable("LoyaltyAccounts");

            builder.HasKey(account => account.Id);

            builder.Property(account => account.CustomerId)
                .IsRequired();

            builder.Property(account => account.MaxPointsPerRedemption)
                .IsRequired();

            builder.HasIndex(account => account.CustomerId)
                .IsUnique();

            builder.HasMany(account => account.PointBatches)
                .WithOne(batch => batch.LoyaltyAccount)
                .HasForeignKey(batch => batch.LoyaltyAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(account => account.Redemptions)
                .WithOne(redemption => redemption.LoyaltyAccount)
                .HasForeignKey(redemption => redemption.LoyaltyAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
