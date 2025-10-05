
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
    public async Task<IActionResult> GetAllUsersWithOrdersAsync()
    {
        var users = await _service.GetAllUsersWithOrdersAsync();
        if (!users.Any())
            return NotFound("Users with orders not found");
        return Ok(users);
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _service.GetUserByIdAsync(id);
        if (user == null)
            return NotFound("User not found");
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto user)
    {
        var rowsAffected = await _service.UpdateUserAsync(id, user);
        if (rowsAffected == 1)
            return Ok("User updated  successfully");
        
        return BadRequest("Unable to update user");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var rowsAffected = await _service.DeleteUserAsync(id);
        if (rowsAffected == 1)
            return Ok("User deleted  successfully");
        
        return BadRequest("Unable to delete user");
    }
}