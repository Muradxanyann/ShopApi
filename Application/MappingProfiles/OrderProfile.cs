using Application.Dto.OrderDto;
using Application.Dto.OrderProductsDto;
using AutoMapper;
using Domain;

namespace Application.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderEntity, OrderResponseDto>()
            .ForMember(dest => dest.Products, 
                opt => opt.MapFrom(src => src.Items));
        CreateMap<OrderCreationDto, OrderEntity>().ReverseMap();
        CreateMap<ProductOrderEntity, OrderProductsInfo>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product!.Name))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Product!.Category))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product!.Price));
    }
}