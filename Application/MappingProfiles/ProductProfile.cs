using AutoMapper;
using Domain;
using Shared.Dto.ProductDto;

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