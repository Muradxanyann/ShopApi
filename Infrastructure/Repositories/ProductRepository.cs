using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IConnectionFactory _connectionFactory;
    public ProductRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async  Task<IEnumerable<ProductEntity?>> GetAllProductsAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  SELECT p.product_id, p.name, p.category, p.price
                  FROM products p
                  ORDER BY p.product_id
                  """;
        return await connection.QueryAsync<ProductEntity>(sql);
    }

    public async Task<ProductEntity?> GetProductByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  SELECT p.product_id, p.name, p.category, p.price
                  FROM products p
                  WHERE p.product_id = @Id";"
                  ORDER BY p.product_id
                  """;
        var product = await connection.QueryAsync<ProductEntity>(sql, new { Id = id });
        return product.SingleOrDefault();
    }

    public async Task<int> CreateProductAsync(ProductEntity user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  INSERT INTO products (name, category, price)
                  VALUES (@Name, @Category, @Price)
                  """;
        return await connection.ExecuteAsync(sql, user);;
    }

    public async Task<int> UpdateProductAsync(int id, ProductEntity user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                    UPDATE products
                    SET name=@Name, category=@Category, price=@Price
                    WHERE product_id=@Id
                  """;
        return await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Name = user.Name,
            Category = user.Category,
            Price = user.Price
        });
    }

    public async Task<int> DeleteProductAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  DELETE FROM products p 
                  WHERE p.product_id = @Id
                  """;                
        return await connection.ExecuteAsync(sql,  new { Id = id });
    }
}