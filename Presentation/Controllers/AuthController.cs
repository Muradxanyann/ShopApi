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
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService,  IUserService userService,  ILogger<AuthController> logger)
    {
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }
    

    [HttpPost("Register")]
    public async Task<IActionResult> CreateUserAsync(UserRegistrationDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user {username}", dto.Username);
        var userId = await _authService.CreateUserAsync(dto, cancellationToken);
        if (userId == 0)
        {
            _logger.LogInformation("User creation failed");
            return BadRequest("Cannot create user");
        }
            
        return Ok($"User created successfully: UserId - {userId}");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(UserLoginDto dto,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request to login from user {username}", dto.Username);
        var login = await _authService.LoginAsync(dto, cancellationToken);
        if (login == null)
        {
            _logger.LogInformation("Login failed");
            return BadRequest("Invalid login or password");
        }
            
        return Ok(login);
    }
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var userId = await _authService.ValidateRefreshTokenAsync(request.RefreshToken,  cancellationToken);
        if (userId == null)
            return Unauthorized("Invalid or expired refresh token");

        var user = await _userService.GetUserByIdAsync(userId.Value,  cancellationToken);
        var newJwt = _authService.GenerateJwtToken(user!);
        var newRefresh = _authService.GenerateRefreshToken();

        await _authService.SaveRefreshTokenAsync(user!.UserId, newRefresh, cancellationToken);

        return Ok(new
        {
            token = newJwt,
            refreshToken = newRefresh,
            expiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }
}