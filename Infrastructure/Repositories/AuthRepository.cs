using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(IConnectionFactory connectionFactory,  ILogger<AuthRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<int> CreateUserAsync(UserEntity entity,  CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user with username {username}", entity.Username);
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                    INSERT INTO users (name, age, phone, email, username, password_hash)
                    VALUES (@name, @age, @phone, @email, @username, @passwordHash)
                    RETURNING user_id
                  """;
        var command = new CommandDefinition(sql, entity, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<UserEntity?> LoginAsync(UserEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Request to login");
        var connection = _connectionFactory.CreateConnection();
        const string sql = """
                             SELECT user_id, name, age, phone, email, username, password_hash, role 
                             FROM users
                             WHERE username = @username
                           """;
        var command = new CommandDefinition(sql, entity, cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<UserEntity>(command);
    }
    public async Task SaveRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Request to save a new refresh token");
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """
                               INSERT INTO refresh_tokens (user_id, token, expires_at)
                               VALUES (@UserId, @Token, @ExpiresAt)
                           """;

        await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            cancellationToken
        });
        _logger.LogInformation("Refresh token saved successfully");
    }

    public async Task<int?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request to validate a new refresh token");
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
                               SELECT user_id 
                               FROM refresh_tokens
                               WHERE token = @Token
                                 AND expires_at > NOW()
                               LIMIT 1
                           """;
        var command = new CommandDefinition(sql, new { Token = token }, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<int?>(command);
    }
}