using System.Data;
using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain;
using Shared;
using Shared.Dto.OrderDto;
using Shared.Dto.UserDto;

namespace Infrastructure.Repositories;
 
public class OrderRepository :  IOrderRepository
{ 
    private readonly IConnectionFactory _connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<OrderEntity?>> GetAllOrdersAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT order_id, user_id, created_at FROM orders";
        var command = new CommandDefinition(sql,cancellationToken);
        return await connection.QueryAsync<OrderEntity>(command);
    }
    
    
    public async Task<IEnumerable<OrderEntity>> GetAllOrdersWithProductsAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
                           SELECT 
                               o.order_id, o.user_id, o.created_at,
                               po.order_id, po.product_id, po.quantity,
                               p.product_id, p.name, p.category, p.price
                           FROM orders o
                           LEFT JOIN order_products po ON o.order_id = po.order_id
                           LEFT JOIN products p ON po.product_id = p.product_id
                           """;

        var orderDictionary = new Dictionary<int, OrderEntity>();
        var command = new CommandDefinition(
            commandText: sql,
            parameters: null, 
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<OrderEntity, ProductOrderEntity, ProductEntity, OrderEntity>(
            command,
            (order, productOrder, product) =>
            {
                if (!orderDictionary.TryGetValue(order.OrderId, out var currentOrder))
                {
                    currentOrder = order;
                    currentOrder.Items = new List<ProductOrderEntity>();
                    orderDictionary.Add(currentOrder.OrderId, currentOrder);
                }

                if (productOrder.ProductId != 0)
                {
                    productOrder.Product = product; 
                    currentOrder.Items.Add(productOrder);
                }

                return currentOrder;
            },
            splitOn: "order_id,product_id"
        );

        return orderDictionary.Values;
    }
    
    public async Task<OrderEntity?> GetOrderWithProductsAsync(int id,  CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

       
        const string sql = """
                           SELECT 
                               o.order_id, o.user_id, o.created_at,
                               po.order_id, po.product_id, po.quantity,
                               p.product_id, p.name, p.category, p.price
                           FROM orders o
                           LEFT JOIN order_products po ON o.order_id = po.order_id
                           LEFT JOIN products p ON po.product_id = p.product_id
                           WHERE o.order_id = @Id
                           """;

        var orderDictionary = new Dictionary<int, OrderEntity>();

        var command = new CommandDefinition(
            commandText: sql,
            parameters: new {Id = id}, 
            cancellationToken: cancellationToken
        );
        var result = await connection.QueryAsync<OrderEntity, ProductOrderEntity, ProductEntity, OrderEntity>(
            command,
            (order, productOrder, product) =>
            {
                if (!orderDictionary.TryGetValue(order.OrderId, out var currentOrder))
                {
                    currentOrder = order;
                    currentOrder.Items = new List<ProductOrderEntity>();
                    orderDictionary.Add(currentOrder.OrderId, currentOrder);
                }

                if (productOrder.ProductId != 0)
                {
                    productOrder.Product = product; 
                    currentOrder.Items.Add(productOrder);
                }

                return currentOrder;
            },
            
            splitOn: "order_id,product_id"
        );

        return orderDictionary.Values.FirstOrDefault();
    }

    public async Task<int> InsertOrderAsync(OrderEntity? order, IDbTransaction transaction,  CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """

                                   INSERT INTO orders (user_id, created_at)
                                   VALUES (@UserId, @CreatedAt)
                                   RETURNING order_id;
                           """;

        var command = new CommandDefinition(
            sql,
            new { order!.UserId, order.CreatedAt },
            transaction,
            cancellationToken: cancellationToken
        );
        return await connection.ExecuteScalarAsync<int>(command);
    }
    
    public async Task<int> CancelOrderAsync(int id , IDbTransaction transaction,   CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "DELETE FROM orders WHERE order_id = @Id RETURNING order_id;";
        var command = new CommandDefinition(
            sql,
            new { Id = id },
            transaction,
            cancellationToken: cancellationToken
        );
        return await connection.ExecuteScalarAsync<int>(command);
    }
}