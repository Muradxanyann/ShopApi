using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain;

namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public AuthRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateUserAsync(UserEntity entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                    INSERT INTO users (name, age, phone, email, username, password_hash)
                    VALUES (@name, @age, @phone, @email, @username, @passwordHash)
                    RETURNING user_id
                  """;
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<UserEntity?> LoginAsync(UserEntity entity)
    {
        var connection = _connectionFactory.CreateConnection();
        const string sql = """
                             SELECT user_id, name, age, phone, email, username, password_hash, role 
                             FROM users
                             WHERE username = @username
                           """;
        return await connection.QueryFirstOrDefaultAsync<UserEntity>(sql, entity);
    }
    public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """
                               INSERT INTO refresh_tokens (user_id, token, expires_at)
                               VALUES (@UserId, @Token, @ExpiresAt)
                           """;

        await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
    }

    public async Task<int?> ValidateRefreshTokenAsync(string token)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
                               SELECT user_id 
                               FROM refresh_tokens
                               WHERE token = @Token
                                 AND expires_at > NOW()
                               LIMIT 1
                           """;

        return await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Token = token });
    }
}