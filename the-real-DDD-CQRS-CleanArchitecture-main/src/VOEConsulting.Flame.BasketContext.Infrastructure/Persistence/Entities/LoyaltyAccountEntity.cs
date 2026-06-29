using System;
using System.Collections.Generic;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class LoyaltyAccountEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<LoyaltyPointsEntryEntity> PointsEntries { get; set; } = new();
    }
}
