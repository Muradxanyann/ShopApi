using Application.Dto.AuthDto;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<int> CreateUserAsync(UserRegistrationDto dto);
    Task<LoginResponseDto?> LoginAsync(UserLoginDto dto);
}