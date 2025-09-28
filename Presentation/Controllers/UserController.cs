using Application;
using Application.UserDto;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        if (!users.Any())
            return NotFound("Users not found");
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            return NotFound("User not found");
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserForCreationDto user)
    {
        try
        {
            var rowsAffected = await _userRepository.CreateUserAsync(user);
            if (rowsAffected == 1)
                return Ok("User created successfully");
            
            return BadRequest("Unable to create user");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto user)
    {
        try
        {
            var rowsAffected = await _userRepository.UpdateUserAsync(id, user);
            if (rowsAffected == 1)
                return Ok("User updated  successfully");
            
            return BadRequest("Unable to update user");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var rowsAffected = await _userRepository.DeleteUserAsync(id);
            if (rowsAffected == 1)
                return Ok("User deleted  successfully");
            
            return BadRequest("Unable to delete user");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }
}