using Application.Dto.AuthDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;

namespace Application.Services;

public class AuthService :  IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public AuthService(IAuthRepository repository, IMapper mapper)
    {
        _authRepository = repository;
        _mapper = mapper;
    }

    public async Task<int> CreateUserAsync(UserRegistrationDto dto)
    {
        var userEntity = _mapper.Map<UserEntity>(dto);
        userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        return await _authRepository.CreateUserAsync(userEntity);
    }

    public Task<LoginResponseDto?> LoginAsync(UserLoginDto dto)
    {
        throw new NotImplementedException();
    }
}