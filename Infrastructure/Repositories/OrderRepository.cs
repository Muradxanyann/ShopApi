using System.Data;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain;

namespace Infrastructure.Repositories;
 
public class OrderRepository :  IOrderRepository
{ 
    private readonly IConnectionFactory _connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<OrderEntity?>> GetAllOrdersAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT order_id, user_id, created_at FROM orders";
        return await connection.QueryAsync<OrderEntity>(sql);
    }

    public async Task<IEnumerable<OrderEntity>> GetAllOrdersWithProductsAsync()
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

        var result = await connection.QueryAsync<OrderEntity, ProductOrderEntity, ProductEntity, OrderEntity>(
            sql,
            (order, productOrder, product) =>
            {
                if (!orderDictionary.TryGetValue(order.OrderId, out var currentOrder))
                {
                    currentOrder = order;
                    currentOrder.Items = new List<ProductOrderEntity>();
                    orderDictionary.Add(currentOrder.OrderId, currentOrder);
                }

                if (productOrder != null && productOrder.ProductId != 0)
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
    
    public async Task<OrderEntity?> GetOrderWithProductsAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

       
        var sql = """
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

        var result = await connection.QueryAsync<OrderEntity, ProductOrderEntity, ProductEntity, OrderEntity>(
            sql,
            (order, productOrder, product) =>
            {
                if (!orderDictionary.TryGetValue(order.OrderId, out var currentOrder))
                {
                    currentOrder = order;
                    currentOrder.Items = new List<ProductOrderEntity>();
                    orderDictionary.Add(currentOrder.OrderId, currentOrder);
                }

                if (productOrder != null && productOrder.ProductId != 0)
                {
                    productOrder.Product = product; 
                    currentOrder.Items.Add(productOrder);
                }

                return currentOrder;
            },
            new{Id = id},
            splitOn: "order_id,product_id"
        );

        return orderDictionary.Values.FirstOrDefault();
    }

    public async Task<int> InsertOrderAsync(OrderEntity? order, IDbTransaction transaction)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """

                                   INSERT INTO orders (user_id, created_at)
                                   VALUES (@UserId, @CreatedAt)
                                   RETURNING order_id;
                           """;

        return await connection.ExecuteScalarAsync<int>(
            sql, 
            new { order.UserId, order.CreatedAt }, 
            transaction
        );
    }
    
    public async Task<int> CancelOrderAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "DELETE FROM orders WHERE order_id = @Id";
        return await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
    }
}