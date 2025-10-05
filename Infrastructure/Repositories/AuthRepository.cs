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
                    VALUES (@name, @age, @phone, @email, @username, @password_hash)
                  """;
        return await connection.ExecuteAsync(sql, new
        {
            name = entity.Name,
            age = entity.Age,
            phone = entity.Phone,
            email = entity.Email,
            username = entity.Username,
            password_hash = entity.PasswordHash
        });
    }

    public Task<UserEntity> LoginAsync(UserEntity entity)
    {
        throw new NotImplementedException();
    }
}