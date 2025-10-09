using Application.Dto.UserDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserService :  IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper mapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users with orders");
        var users = await _userRepository.GetAllUsersWithOrdersAsync(cancellationToken);
        var response =  _mapper.Map<IEnumerable<UserResponseDto?>>(users);
        return response;
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user with id {id}", id);
        return await _userRepository.GetUserByIdAsync(id,  cancellationToken);
    }

    public async Task<int> UpdateUserAsync(int id, UserUpdateDto user,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user with id {id}", id);
        var userEntity = _mapper.Map<UserEntity>(user);
        return await _userRepository.UpdateUserAsync(id, userEntity,  cancellationToken);
    }

    public async Task<int> DeleteUserAsync(int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user with id {id}", id);
        return await _userRepository.DeleteUserAsync(id, cancellationToken);
    }
    
    
    
}