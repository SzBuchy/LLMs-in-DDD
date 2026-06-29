using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Configurations
{
    public class LoyaltyPointsEntryConfiguration : IEntityTypeConfiguration<LoyaltyPointsEntryEntity>
    {
        public void Configure(EntityTypeBuilder<LoyaltyPointsEntryEntity> builder)
        {
            builder.ToTable("LoyaltyPointsEntries");

            builder.HasKey(pe => pe.Id);

            builder.Property(pe => pe.Amount)
                .IsRequired();

            builder.Property(pe => pe.UsedAmount)
                .IsRequired();

            builder.Property(pe => pe.EarnedAtUtc)
                .IsRequired();

            builder.Property(pe => pe.ExpiresAtUtc)
                .IsRequired();
        }
    }
}
