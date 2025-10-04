using Application.Dto.OrderDto;
using AutoMapper;
using Domain;

namespace Application.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderEntity, OrderResponseDto>().ReverseMap();
        CreateMap<OrderCreationDto, OrderEntity>().ReverseMap();

    }
}