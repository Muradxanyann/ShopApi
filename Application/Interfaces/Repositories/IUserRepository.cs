using Domain;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity?>> GetAllUsersWithOrdersAsync(CancellationToken cancellationToken = default);
    Task<UserEntity?>  GetUserByIdAsync(int id,  CancellationToken cancellationToken = default);
    Task<int> UpdateUserAsync(int id, UserEntity user,  CancellationToken cancellationToken = default);
    Task<int> DeleteUserAsync(int id,  CancellationToken cancellationToken = default);
}

