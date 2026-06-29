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

            builder.HasKey(la => la.Id);

            builder.Property(la => la.CustomerId)
                .IsRequired();

            builder.HasIndex(la => la.CustomerId)
                .IsUnique();

            builder.HasMany(la => la.PointsEntries)
                .WithOne()
                .HasForeignKey(pe => pe.LoyaltyAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
