using Domain;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity?>> GetAllUsersWithOrdersAsync();
    Task<UserEntity?>  GetUserByIdAsync(int id);
    Task<int> UpdateUserAsync(int id, UserEntity user);
    Task<int> DeleteUserAsync(int id);
}

