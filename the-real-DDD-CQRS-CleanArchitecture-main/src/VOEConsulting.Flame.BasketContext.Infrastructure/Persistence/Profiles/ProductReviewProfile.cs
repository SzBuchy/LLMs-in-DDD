using AutoMapper;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

public class ProductReviewMappingProfile : Profile
{
    public ProductReviewMappingProfile()
    {
        // Domain -> Entity mapping
        CreateMap<ProductReview, ProductReviewEntity>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name));

        // Entity -> Domain mapping
        CreateMap<ProductReviewEntity, ProductReview>()
            .ConstructUsing(entity => ProductReview.Create(
                Id<Customer>.FromGuid(entity.CustomerId),
                entity.ProductId,
                entity.Rating,
                entity.Content,
                Id<ProductReview>.FromGuid(entity.Id)))
            .AfterMap((entity, domain) =>
            {
                var status = ReviewStatus.FromName(entity.Status);

                if (status == ReviewStatus.Published)
                    domain.Publish();
                else if (status == ReviewStatus.Rejected)
                    domain.Reject();

                // Reconstructing state through domain methods raises domain events
                // that don't represent anything new happening - discard them.
                domain.ClearEvents();
            });
    }
}
