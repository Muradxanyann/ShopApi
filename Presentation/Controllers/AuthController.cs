using Application.Dto.AuthDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService,  IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> CreateUserAsync(UserRegistrationDto dto)
    {
        var userId = await _authService.CreateUserAsync(dto);
        if (userId == 0)
            return BadRequest("Cannot create user");
        return Ok($"User created successfully: UserId - {userId}");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(UserLoginDto dto)
    {
        var login = await _authService.LoginAsync(dto);
        if (login == null)
            return BadRequest("Invalid login or password");
        return Ok(login);
    }
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var userId = await _authService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (userId == null)
            return Unauthorized("Invalid or expired refresh token");

        var user = await _userService.GetUserByIdAsync(userId.Value);
        var newJwt = _authService.GenerateJwtToken(user!);
        var newRefresh = _authService.GenerateRefreshToken();

        await _authService.SaveRefreshTokenAsync(user!.UserId, newRefresh);

        return Ok(new
        {
            token = newJwt,
            refreshToken = newRefresh,
            expiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }
}