using Application;
using Application.UserDto;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    
    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration; 
    }
    
    public async Task<IEnumerable<User?>> GetAllUsersAsync()
    {
        await using var connection = GetConnection();
        var users = 
            await connection.QueryAsync<User>("SELECT user_id AS Id, name, age, phone, email FROM users " +
                                               "ORDER BY Id ASC");
        return users.ToList();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        await using var connection = GetConnection();
        var user = await 
            connection.QueryAsync<User>("SELECT user_id AS Id, name, age, phone, email" + 
                                         " FROM users WHERE user_id = @id", new { id });
        return  user.FirstOrDefault();
    }

    public async Task<int> CreateUserAsync(UserForCreationDto user)
    {
        await using var connection = GetConnection();
        const string sql = """
                               INSERT INTO users (name, age, phone, email)
                               VALUES (@Name, @Age, @Phone, @Email)
                           """;

        return await connection.ExecuteAsync(sql, user);
    }

    public async Task<int> UpdateUserAsync(int id, UserForUpdateDto user)
    {
        await using var connection = GetConnection();
        const string sql = "UPDATE users SET name = @Name, age = @Age, phone = @Phone, email = @Email " +
                           "WHERE user_id = @Id";
        return await connection.ExecuteAsync(sql, new
        {
            Id = id,
            user.Name,
            user.Age,
            user.Phone,
            user.Email
        });
    }

    public async Task<int> DeleteUserAsync(int id)
    {
        await using var connection = GetConnection();
        const string sql = "DELETE FROM users WHERE user_id = @id";
        return await connection.ExecuteAsync(sql, new {id});
    }
    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}