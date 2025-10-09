
using Application.Dto.UserDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Users")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UserController> _logger;
    
    public UserController(IUserService service,  ILogger<UserController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUsersWithOrdersAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users with orders");
        var users = await _service.GetAllUsersWithOrdersAsync(cancellationToken);
        if (!users.Any())
        {
            _logger.LogInformation("No users found");
            return NotFound("Users with orders not found");
        }
            
        return Ok(users);
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user with id {id}", id);
        var user = await _service.GetUserByIdAsync(id,  cancellationToken);
        if (user == null)
        {
            _logger.LogInformation("User with id {id} not found", id);
            return NotFound("User not found");
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto user,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user with id {id}", id);
        var rowsAffected = await _service.UpdateUserAsync(id, user,  cancellationToken);
        if (rowsAffected == 1)
        {
            _logger.LogInformation("User with id {id} updated", id);
            return Ok("User updated  successfully");
        }
        
        return BadRequest("Unable to update user");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user with id {id}", id);
        var rowsAffected = await _service.DeleteUserAsync(id, cancellationToken);
        if (rowsAffected == 1)
        {
            _logger.LogInformation("User with id {id} deleted", id);
            return Ok("User deleted  successfully");
        }
        
        return BadRequest("Unable to delete user");
    }
}