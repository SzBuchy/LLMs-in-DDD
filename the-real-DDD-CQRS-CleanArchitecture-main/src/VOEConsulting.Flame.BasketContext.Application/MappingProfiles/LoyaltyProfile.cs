using AutoMapper;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos;

namespace VOEConsulting.Flame.BasketContext.Application.MappingProfiles
{
    public class LoyaltyProfile : Profile
    {
        public LoyaltyProfile()
        {
            CreateMap<LoyaltyAccount, LoyaltyAccountDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value))
                .ForMember(dest => dest.AvailablePointsBalance, opt => opt.MapFrom(src => src.GetAvailablePointsBalance(DateTimeOffset.UtcNow)))
                .ForMember(dest => dest.PointsEntries, opt => opt.MapFrom(src => src.PointsEntries));

            CreateMap<LoyaltyPointsEntry, LoyaltyPointsEntryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.UsedAmount, opt => opt.MapFrom(src => src.UsedAmount))
                .ForMember(dest => dest.EarnedAtUtc, opt => opt.MapFrom(src => src.EarnedAtUtc))
                .ForMember(dest => dest.ExpiresAtUtc, opt => opt.MapFrom(src => src.ExpiresAtUtc))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => DateTimeOffset.UtcNow >= src.ExpiresAtUtc));
        }
    }
}
