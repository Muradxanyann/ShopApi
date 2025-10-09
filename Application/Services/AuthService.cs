using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Dto.AuthDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Application.Services;

public class AuthService :  IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    
    public AuthService(IAuthRepository repository, IConfiguration configuration, IMapper mapper)
    {
        _authRepository = repository;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<int> CreateUserAsync(UserRegistrationDto dto, CancellationToken cancellationToken)
    {
        var userEntity = _mapper.Map<UserEntity>(dto);
        userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        return await _authRepository.CreateUserAsync(userEntity, cancellationToken);
    }

    public async Task<LoginResponseDto?> LoginAsync(UserLoginDto dto,  CancellationToken cancellationToken)
    {
        var toUserEntity = _mapper.Map<UserEntity>(dto);
        var userEntity = await _authRepository.LoginAsync(toUserEntity, cancellationToken);
        if (userEntity == null)
            return null;
        
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, userEntity.PasswordHash))
            return null;

        var accessToken = GenerateJwtToken(userEntity);
        var refreshToken = GenerateRefreshToken();
        
        
        await SaveRefreshTokenAsync(userEntity.UserId, refreshToken, cancellationToken);
        return new LoginResponseDto
        {
            UserId = userEntity.UserId,
            Username = userEntity.Username,
            Role = userEntity.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        
    }
    public string GenerateJwtToken(UserEntity userEntity)
    {
        var key = _configuration["JwtSettings:Key"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userEntity.UserId.ToString()),
            new Claim(ClaimTypes.Name, userEntity.Username),
            new Claim(ClaimTypes.Email, userEntity.Email),
            new Claim(ClaimTypes.Role, userEntity.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task SaveRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken)
    { 
        await _authRepository.SaveRefreshTokenAsync(userId, refreshToken,  cancellationToken);
    }

    public Task<int?> ValidateRefreshTokenAsync(string token,  CancellationToken cancellationToken)
    {
        return _authRepository.ValidateRefreshTokenAsync(token,  cancellationToken);
    }
}