using Application.Dto.AuthDto;
using Application.Dto.UserDto;
using AutoMapper;
using Domain;

namespace Application.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserResponseDto>().ReverseMap();
        CreateMap<UserEntity, UserUpdateDto>().ReverseMap();

        CreateMap<UserCreationDto, UserEntity>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());

        CreateMap<UserRegistrationDto, UserEntity>().ReverseMap();
    }
}