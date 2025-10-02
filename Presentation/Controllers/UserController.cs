
using Application.Dto.UserDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    
    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _service.GetAllUsersAsync();
        if (!users.Any())
            return NotFound("Users not found");
        return Ok(users);
    }
    
    [HttpGet("orders")]
    public async Task<IActionResult> GetAllUsersWithOrdersAsync()
    {
        var users = await _service.GetAllUsersWithOrdersAsync();
        if (!users.Any())
            return NotFound("Users not found");
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

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserForCreationDto user)
    {
        var rowsAffected = await _service.CreateUserAsync(user);
        if (rowsAffected == 1)
            return Ok("User created successfully");
        
        return BadRequest("Unable to create user");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto user)
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