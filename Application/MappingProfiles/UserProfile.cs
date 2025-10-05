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

       
        CreateMap<UserRegistrationDto, UserEntity>().ReverseMap();
        CreateMap<UserLoginDto, UserEntity>().ReverseMap();
    }
}