using AutoMapper;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

public class LoyaltyAccountMappingProfile : Profile
{
    public LoyaltyAccountMappingProfile()
    {
        // Domain -> Entity mappings
        CreateMap<LoyaltyAccount, LoyaltyAccountEntity>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value))
            .ForMember(dest => dest.PointsLots, opt => opt.MapFrom(src => src.PointsLots));

        CreateMap<LoyaltyPointsLot, LoyaltyPointsLotEntity>();

        // Entity -> Domain mapping
        CreateMap<LoyaltyAccountEntity, LoyaltyAccount>()
            .ConstructUsing(entity => LoyaltyAccount.Rehydrate(
                Id<LoyaltyAccount>.FromGuid(entity.Id),
                Id<Customer>.FromGuid(entity.CustomerId),
                entity.PointsLots.Select(lot => LoyaltyPointsLot.Create(lot.Points, lot.EarnedAtUtc))));
    }
}
