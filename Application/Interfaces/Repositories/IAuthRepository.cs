using Domain;

namespace Application.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<int> CreateUserAsync(UserEntity entity);
    Task<UserEntity> LoginAsync(UserEntity entity);
}
