using Application.Dto.UserDto;
using Application.Interfaces;
using Domain;

namespace Application.Services;

public class UserService :  IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserEntity?>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync()
    {
        return await _userRepository.GetAllUsersWithOrdersAsync();
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<int> CreateUserAsync(UserForCreationDto user)
    {
        return await _userRepository.CreateUserAsync(user);
    }

    public async Task<int> UpdateUserAsync(int id, UserForUpdateDto user)
    {
        return await _userRepository.UpdateUserAsync(id, user);
    }

    public async Task<int> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteUserAsync(id);
    }
    
    
    
}