using Application.Dto.UserDto;
using Domain;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync();
    Task<UserEntity?>  GetUserByIdAsync(int id);
    Task<int> UpdateUserAsync(int id, UserUpdateDto user);
    Task<int> DeleteUserAsync(int id);
}