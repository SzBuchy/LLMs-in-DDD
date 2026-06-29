using AutoMapper;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Profiles
{
    public class LoyaltyProfile : Profile
    {
        public LoyaltyProfile()
        {
            // Domain -> Entity
            CreateMap<LoyaltyAccount, LoyaltyAccountEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value))
                .ForMember(dest => dest.PointsEntries, opt => opt.MapFrom(src => src.PointsEntries));

            CreateMap<LoyaltyPointsEntry, LoyaltyPointsEntryEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.UsedAmount, opt => opt.MapFrom(src => src.UsedAmount))
                .ForMember(dest => dest.EarnedAtUtc, opt => opt.MapFrom(src => src.EarnedAtUtc))
                .ForMember(dest => dest.ExpiresAtUtc, opt => opt.MapFrom(src => src.ExpiresAtUtc));

            // Entity -> Domain
            CreateMap<LoyaltyPointsEntryEntity, LoyaltyPointsEntry>()
                .ConstructUsing(src => LoyaltyPointsEntry.Reconstitute(
                    Id<LoyaltyPointsEntry>.FromGuid(src.Id),
                    src.Amount,
                    src.UsedAmount,
                    src.EarnedAtUtc,
                    src.ExpiresAtUtc
                ));

            CreateMap<LoyaltyAccountEntity, LoyaltyAccount>()
                .ConstructUsing((src, ctx) => LoyaltyAccount.Reconstitute(
                    Id<LoyaltyAccount>.FromGuid(src.Id),
                    Id<Customer>.FromGuid(src.CustomerId),
                    ctx.Mapper.Map<List<LoyaltyPointsEntry>>(src.PointsEntries)
                ));
        }
    }
}
