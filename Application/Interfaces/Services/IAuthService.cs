using Application.Dto.AuthDto;
using Domain;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<int> CreateUserAsync(UserRegistrationDto dto, CancellationToken cancellationToken = default);
    Task<LoginResponseDto?> LoginAsync(UserLoginDto dto, CancellationToken cancellationToken = default);
    
    string GenerateJwtToken(UserEntity userEntity);
    string GenerateRefreshToken();
    Task SaveRefreshTokenAsync(int userId, string refreshToken,  CancellationToken cancellationToken = default);
    Task<int?> ValidateRefreshTokenAsync(string token,  CancellationToken cancellationToken = default);
}