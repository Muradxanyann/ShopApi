using Application.Dto.AuthDto;
using Application.Interfaces.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> CreateUserAsync(UserRegistrationDto dto)
    {
        var create = await _authService.CreateUserAsync(dto);
        if (create != 1)
            return BadRequest("Cannot create user");
        return Ok(create);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(UserLoginDto dto)
    {
        var login = await _authService.LoginAsync(dto);
        if (login == null)
            return Unauthorized();
        return Ok(login);
    }
}