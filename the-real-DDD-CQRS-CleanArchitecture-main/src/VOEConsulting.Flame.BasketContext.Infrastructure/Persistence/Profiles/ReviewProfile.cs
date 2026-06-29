using AutoMapper;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Persistence.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            // Domain -> Entity
            CreateMap<Review, ReviewEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.Value))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

            // Entity -> Domain
            CreateMap<ReviewEntity, Review>()
                .ConstructUsing(src => Review.Reconstitute(
                    Id<Review>.FromGuid(src.Id),
                    Id<Customer>.FromGuid(src.CustomerId),
                    Id<Product>.FromGuid(src.ProductId),
                    src.Rating,
                    src.Content,
                    (ReviewStatus)src.Status
                ));
        }
    }
}
