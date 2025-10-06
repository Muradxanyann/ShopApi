using System.Data;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain;

namespace Infrastructure.Repositories;

public class OrderProductRepository : IOrderProductRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public OrderProductRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> InsertOrderProductAsync(ProductOrderEntity entity, IDbTransaction transaction =  null!)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """
                             INSERT INTO order_products (order_Id, product_Id, quantity)
                             VALUES (@orderId, @productId, @quantity)
                           """;
        return await connection.ExecuteAsync(sql, new
        {
            orderId = entity.OrderId,
            productId = entity.ProductId,
            quantity = entity.Quantity
        }, transaction);

    }

    public async Task<int> DeleteOrderProductAsync(int orderId, IDbTransaction transaction = null!)
    {
        var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM order_products WHERE order_id = @orderId RETURNING order_id;";
        return await connection.ExecuteScalarAsync<int>(sql, new { orderId }, transaction);
    }
}