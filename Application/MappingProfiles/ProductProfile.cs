using Application.Dto.ProductDto;
using AutoMapper;
using Domain;

namespace Application.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductEntity, ProductResponseDto>().ReverseMap();
        CreateMap<ProductEntity, ProductUpdateDto>().ReverseMap();
        CreateMap<ProductEntity, ProductCreationDto>().ReverseMap();
    }
}