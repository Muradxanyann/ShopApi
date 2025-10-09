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

    public async  Task<IEnumerable<ProductEntity?>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  SELECT p.product_id, p.name, p.category, p.price
                  FROM products p
                  ORDER BY p.product_id
                  """;
        var command = new CommandDefinition(sql, cancellationToken : cancellationToken);
        return await connection.QueryAsync<ProductEntity>(command);
    }

    public async Task<ProductEntity?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  SELECT p.product_id, p.name, p.category, p.price
                  FROM products p
                  WHERE p.product_id = @Id
                  ORDER BY p.product_id
                  """;
        var command = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken
        );
        var product = await connection.QueryAsync<ProductEntity>(command);
        return product.SingleOrDefault();
    }

    public async Task<int> CreateProductAsync(ProductEntity user,  CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  INSERT INTO products (name, category, price)
                  VALUES (@Name, @Category, @Price)
                  """;
        var command = new CommandDefinition(
            sql,
            user,
            cancellationToken: cancellationToken
        );
        return await connection.ExecuteAsync(command);;
    }

    public async Task<int> UpdateProductAsync(int id, ProductEntity user,  CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                    UPDATE products
                    SET name=@Name, category=@Category, price=@Price
                    WHERE product_id=@Id
                  """;
        var command = new CommandDefinition(
            sql,
            new
            {
                Id = id,
                Name = user.Name,
                Category = user.Category,
                Price = user.Price
            },
            cancellationToken: cancellationToken
        );
        return await connection.ExecuteAsync(command);
    }

    public async Task<int> DeleteProductAsync(int id,  CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = """
                  DELETE FROM products p 
                  WHERE p.product_id = @Id
                  """;               
        var command = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken
        );
        return await connection.ExecuteAsync(command);
    }
}