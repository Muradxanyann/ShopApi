
using Application.Dto.OrderDto;
using Application.Dto.OrderProductsDto;

using Application.Dto.UserDto;
using Application.Interfaces;
using Dapper;
using Domain;


// ReSharper disable once CheckNamespace
namespace Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public UserRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<IEnumerable<UserEntity?>> GetAllUsersAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        const string sql = """
                                SELECT user_id, name, age, phone, email FROM users
                                ORDER BY user_id ASC
                           """;
        
        var users = await connection.QueryAsync<UserEntity>(sql);
        return users.ToList();
    }

    public async Task<IEnumerable<UserResponseDto?>> GetAllUsersWithOrdersAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        const string sql = """

                                   SELECT u.user_id, u.name, u.age, u.email,
                                          o.order_id, o.created_at,
                                          po.product_id, po.quantity,
                                          p.name, p.category, p.price
                                   FROM users AS u
                                   JOIN orders AS o ON u.user_id = o.user_id
                                   JOIN order_products AS po ON o.order_id = po.order_id
                                   JOIN products AS p ON po.product_id = p.product_id
                                   ORDER BY u.user_id, o.order_id
                           """;
        var userDictionary = new Dictionary<int, UserResponseDto>();
        var result = await connection.QueryAsync<UserResponseDto, OrderResponseDto, OrderProductsDto, UserResponseDto>(
            sql,
            (user, order, productInfo) =>
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
                    currentOrder.Products = new List<OrderProductsDto>();
                    currentUser.Orders!.Add(currentOrder);
                }
                
                currentOrder.Products.Add(productInfo);

                return currentUser;
            },
            splitOn: "order_id,product_id"
        );

        return userDictionary.Values.ToList();
        
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var user = await 
            connection.QueryAsync<UserEntity>("SELECT user_id, name, age, phone, email" + 
                                         " FROM users WHERE user_id = @id", new { id });
        return  user.FirstOrDefault();
    }

    public async Task<int> CreateUserAsync(UserForCreationDto user)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        const string sql = """
                               INSERT INTO users (name, age, phone, email)
                               VALUES (@Name, @Age, @Phone, @Email)
                           """;

        return await connection.ExecuteAsync(sql, user);
        
    }

    public async Task<int> UpdateUserAsync(int id, UserForUpdateDto user)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
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
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return await connection.ExecuteAsync("DELETE FROM users WHERE user_id = @id", new {id});
    }
    
}