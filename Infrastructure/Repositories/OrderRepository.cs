using Application;
using Application.Dto.OrderDto;
using Application.Dto.OrderProductsDto;
using Application.Interfaces;
using Dapper;

namespace Infrastructure.Repositories;
 
public class OrderRepository :  IOrderRepository
{ 
    private readonly IConnectionFactory _connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<OrderResponseDto?>> GetAllOrdersWithProductsAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
                           SELECT o.order_id, o.user_id, o.created_at,
                           	  po.product_id, po.quantity, 
                           	  p.name, p.category, p.price
                           FROM orders o
                           LEFT JOIN order_products po ON po.order_id = o.order_id
                           LEFT JOIN products p ON po.product_id = p.product_id
                           """;
        var result = await connection.QueryAsync<OrderResponseDto, OrderProductsDto, OrderResponseDto>(
            sql,
            (order, product) =>
            {
                order.Products.Add(product);
                return order;
            },
            splitOn: "product_id"
        );
        return result;
    }
}