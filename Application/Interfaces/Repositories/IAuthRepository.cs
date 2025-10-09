using Domain;

namespace Application.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<int> CreateUserAsync(UserEntity entity, CancellationToken cancellationToken = default);
    Task<UserEntity?> LoginAsync(UserEntity entity, CancellationToken cancellationToken = default);
    Task SaveRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<int?> ValidateRefreshTokenAsync(string token,  CancellationToken cancellationToken = default);
}
