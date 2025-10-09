using Application.Interfaces;
using Application.Interfaces.Repositories;
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
    

    public async Task<IEnumerable<UserEntity?>> GetAllUsersWithOrdersAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        const string sql = """
                               SELECT u.user_id, u.name, u.age, u.email, u.username, u.role,
                                      o.order_id, o.created_at,
                                      po.product_id, po.quantity,
                                      p.name, p.category, p.price
                               FROM users AS u
                               JOIN orders AS o ON u.user_id = o.user_id
                               JOIN order_products AS po ON o.order_id = po.order_id
                               JOIN products AS p ON po.product_id = p.product_id
                               ORDER BY u.user_id, o.order_id
                           """;

        var userDictionary = new Dictionary<int, UserEntity>();

        
        var command = new CommandDefinition(
            commandText: sql,
            parameters: null, 
            cancellationToken: cancellationToken
        );

        // передаём command в QueryAsync
        var result = await connection.QueryAsync<UserEntity, OrderEntity, ProductOrderEntity, UserEntity>(
            command,
            (user, order, productInfo) =>
            {
                if (!userDictionary.TryGetValue(user.UserId, out var currentUser))
                {
                    currentUser = user;
                    currentUser.Orders = new List<OrderEntity>();
                    userDictionary.Add(currentUser.UserId, currentUser);
                }

                var currentOrder = currentUser.Orders!.FirstOrDefault(o => o.OrderId == order.OrderId);
                if (currentOrder == null)
                {
                    currentOrder = order;
                    currentOrder.Items = new List<ProductOrderEntity>();
                    currentUser.Orders!.Add(currentOrder);
                }

                currentOrder.Items.Add(productInfo);

                return currentUser;
            },
            splitOn: "order_id,product_id"
        );

        return userDictionary.Values.ToList();
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        const string sql = """
                               SELECT user_id, name, age, phone, email, username, role
                               FROM users WHERE user_id = @id
                           """;
        var command = new CommandDefinition(sql, new {id}, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<UserEntity>(command);

    }

    public async Task<int> UpdateUserAsync(int id, UserEntity user,  CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        const string sql = "UPDATE users SET name = @Name, age = @Age, phone = @Phone, email = @Email " +
                           "WHERE user_id = @Id";
        var command = new CommandDefinition(sql, new
        {
            Id = id,
            user.Name,
            user.Age,
            user.Phone,
            user.Email
        }, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command);
    }

    public async Task<int> DeleteUserAsync(int id,  CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var sql = "DELETE FROM users WHERE user_id = @id";
        var command = new CommandDefinition(sql, new {id}, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command);
    }
    
}