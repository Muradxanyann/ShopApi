using Application.Dto.UserDto;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;

namespace Application.Services;

public class UserService :  IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository,  IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync()
    {
        var users = await _userRepository.GetAllUsersWithOrdersAsync();
        var response =  _mapper.Map<IEnumerable<UserResponseDto?>>(users);
        return response;
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<int> UpdateUserAsync(int id, UserUpdateDto user)
    {
        var userEntity = _mapper.Map<UserEntity>(user);
        return await _userRepository.UpdateUserAsync(id, userEntity);
    }

    public async Task<int> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteUserAsync(id);
    }
    
    
    
}