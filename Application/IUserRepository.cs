using Application.UserDto;
using Domain;

namespace Application;

public interface IUserRepository
{
    Task<IEnumerable<User?>> GetAllUsersAsync();
    Task<User?>  GetUserByIdAsync(int id);
    Task<int> CreateUserAsync(UserForCreationDto user);
    Task<int> UpdateUserAsync(int id, UserForUpdateDto user);
    Task<int> DeleteUserAsync(int id);
}