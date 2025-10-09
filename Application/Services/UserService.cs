using Application.Dto.UserDto;
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
    
    public async Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersWithOrdersAsync(cancellationToken);
        var response =  _mapper.Map<IEnumerable<UserResponseDto?>>(users);
        return response;
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserByIdAsync(id,  cancellationToken);
    }

    public async Task<int> UpdateUserAsync(int id, UserUpdateDto user,  CancellationToken cancellationToken)
    {
        var userEntity = _mapper.Map<UserEntity>(user);
        return await _userRepository.UpdateUserAsync(id, userEntity,  cancellationToken);
    }

    public async Task<int> DeleteUserAsync(int id,  CancellationToken cancellationToken)
    {
        return await _userRepository.DeleteUserAsync(id, cancellationToken);
    }
    
    
    
}