using Application.Dto.AuthDto;
using Domain;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<int> CreateUserAsync(UserRegistrationDto dto);
    Task<LoginResponseDto?> LoginAsync(UserLoginDto dto);
    
    string GenerateJwtToken(UserEntity userEntity);
    string GenerateRefreshToken();
    Task SaveRefreshTokenAsync(int userId, string refreshToken);
    Task<int?> ValidateRefreshTokenAsync(string token);
}