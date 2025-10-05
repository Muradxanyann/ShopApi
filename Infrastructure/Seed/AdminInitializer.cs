using Dapper;
using Domain;
using Microsoft.Extensions.Options;
using IConnectionFactory = Application.Interfaces.IConnectionFactory;

namespace Infrastructure.Seed;

public class AdminInitializer
{ 
    private readonly AdminSettings _adminSettings;
    private readonly IConnectionFactory  _connectionFactory;


    public AdminInitializer(IConnectionFactory connectionFactory, IOptions<AdminSettings> adminOptions)
    {
        _connectionFactory = connectionFactory;
        _adminSettings = adminOptions.Value;
    }

    public async Task InitializeAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "  SELECT * FROM users WHERE username = @Username";

        var existingUser = await connection.QueryFirstOrDefaultAsync<UserEntity>(
            sql, new { _adminSettings.Username, _adminSettings.Email });

        if (existingUser == null)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(_adminSettings.Password);

            const string sql2 = """
                                INSERT INTO users (username, email, password_hash, role, name, phone, age)
                                                        VALUES (@Username, @Email, @PasswordHash, @Role, @Name, @Phone, @Age)
                                """;

            await connection.ExecuteAsync(sql2, new
            {
                _adminSettings.Username,
                _adminSettings.Email,
                PasswordHash = hashedPassword,
                Role = "Admin",
                Name = _adminSettings.Username,
                Phone = "000000000",
                Age = 0
            });
        }
    }
}