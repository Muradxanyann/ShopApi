
using Application.Dto.UserDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Users")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    
    public UserController(IUserService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUsersWithOrdersAsync(CancellationToken cancellationToken)
    {
        var users = await _service.GetAllUsersWithOrdersAsync(cancellationToken);
        if (!users.Any())
            return NotFound("Users with orders not found");
        return Ok(users);
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id,  CancellationToken cancellationToken)
    {
        var user = await _service.GetUserByIdAsync(id,  cancellationToken);
        if (user == null)
            return NotFound("User not found");
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto user,  CancellationToken cancellationToken)
    {
        var rowsAffected = await _service.UpdateUserAsync(id, user,  cancellationToken);
        if (rowsAffected == 1)
            return Ok("User updated  successfully");
        
        return BadRequest("Unable to update user");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id,  CancellationToken cancellationToken)
    {
        var rowsAffected = await _service.DeleteUserAsync(id);
        if (rowsAffected == 1)
            return Ok("User deleted  successfully");
        
        return BadRequest("Unable to delete user");
    }
}