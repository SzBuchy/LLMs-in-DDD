using AutoMapper;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Infrastructure.Entities;

public class BasketMappingProfile : Profile
{
    public BasketMappingProfile()
    {
        // Domain -> Entity mappings
        CreateMap<Basket, BasketEntity>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
            .ForMember(dest => dest.BasketItems, opt => opt.MapFrom(src => src.BasketItems.SelectMany(kvp => kvp.Value.Items)));

        CreateMap<BasketItem, BasketItemEntity>()
             .ForMember(dest => dest.PricePerUnit, opt => opt.MapFrom(src => src.Quantity.PricePerUnit))
            .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.Seller.Id))
            .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src.Seller)); // Avoid circular mapping

        CreateMap<Seller, SellerEntity>();
        CreateMap<Customer, CustomerEntity>();

        // Entity -> Domain mappings
        CreateMap<BasketEntity, Basket>()
            .ConstructUsing((entity, context) =>
            {
                var customer = context.Mapper.Map<Customer>(entity.Customer);
                var basketItems = entity.BasketItems?.Select(context.Mapper.Map<BasketItem>) ?? [];
                return Basket.Rehydrate(
                    entity.Id,
                    entity.TaxPercentage,
                    entity.TotalAmount,
                    customer,
                    entity.CouponId,
                    basketItems);
            })
            .ForMember(dest => dest.BasketItems, opt => opt.Ignore());

        CreateMap<BasketItemEntity, BasketItem>()
            .ConstructUsing((entity, context) =>
            {
                var seller = context.Mapper.Map<Seller>(entity.Seller);
                var quantity = Quantity.Create(entity.QuantityValue, entity.QuantityLimit, entity.PricePerUnit);
                return BasketItem.Create(entity.Name, quantity, entity.ImageUrl, seller, entity.Id);
            });

        CreateMap<SellerEntity, Seller>()
            .ConstructUsing(entity => Seller.Create(entity.Name, entity.Rating, entity.ShippingLimit, entity.ShippingCost, entity.Id));

        CreateMap<CustomerEntity, Customer>()
            .ConstructUsing(entity => Customer.Create(entity.IsEliteMember, entity.Id));
    }

}
