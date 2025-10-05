using Domain;

namespace Application.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<int> CreateUserAsync(UserEntity entity);
    Task<UserEntity?> LoginAsync(UserEntity entity);
    Task SaveRefreshTokenAsync(int userId, string refreshToken);
    Task<int?> ValidateRefreshTokenAsync(string token);
}
