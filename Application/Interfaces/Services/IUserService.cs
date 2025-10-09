using Application.Dto.UserDto;
using Domain;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync(CancellationToken cancellationToken = default);
    Task<UserEntity?>  GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> UpdateUserAsync(int id, UserUpdateDto user, CancellationToken cancellationToken = default);
    Task<int> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
}