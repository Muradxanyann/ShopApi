using Application;
using Application.OrderDto;
using Application.ProductDto;
using Application.UserDto;
using Dapper;
using Domain;
using Microsoft.Extensions.Configuration;
using Npgsql;

// ReSharper disable once CheckNamespace
namespace Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    
    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration; 
    }
    
    public async Task<IEnumerable<UserEntity?>> GetAllUsersAsync()
    {
        await using var connection = GetConnection();
        var users = 
            await connection.QueryAsync<UserEntity>("SELECT user_id AS Id, name, age, phone, email FROM users " +
                                               "ORDER BY Id ASC");
        return users.ToList();
    }

    public async Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync()
    {
        await using var connection = GetConnection();
        const string sql = """

                                   SELECT u.user_id AS UserId, u.name AS Name, u.age AS Age, u.email AS Email,
                                          o.order_id AS OrderId, o.quantity AS Quantity, o.created_at AS CreatedAt,
                                          p.product_id AS ProductId, p.name AS Name, p.category AS Category, p.price AS Price
                                   FROM users AS u
                                   JOIN orders AS o ON u.user_id = o.user_id
                                   JOIN products AS p ON o.product_id = p.product_id
                                   ORDER BY u.user_id, o.order_id
                           """;
        var userDictionary = new Dictionary<int, UserResponseDto>();
        var result = await connection.QueryAsync<UserResponseDto, OrderResponseDto, ProductResponseDto, UserResponseDto>(
            sql,
            (user, order, product) =>
            {
                //for user
                if (!userDictionary.TryGetValue(user.UserId, out var currentUser))
                {
                    currentUser = user;
                    currentUser.Orders = new List<OrderResponseDto>();
                    userDictionary.Add(currentUser.UserId, currentUser);
                }

                /*
                    for products with the same id, it helps to avoid getting duplicates
                    when one order can have multiple products
                */
                
                var currentOrder = currentUser.Orders!.FirstOrDefault(o => o.OrderId == order.OrderId);
                if (currentOrder == null)
                {
                    currentOrder = order;
                    currentOrder.Products = new List<ProductResponseDto>();
                    currentUser.Orders!.Add(currentOrder);
                }
                
                currentOrder.Products.Add(product);

                return currentUser;
            },
            splitOn: "OrderId,ProductId"
        );

        return userDictionary.Values.ToList();
        
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        await using var connection = GetConnection();
        var user = await 
            connection.QueryAsync<UserEntity>("SELECT user_id AS Id, name, age, phone, email" + 
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