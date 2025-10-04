using Domain;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<ProductEntity?>> GetAllProductsAsync();
    Task<ProductEntity?>  GetProductByIdAsync(int id);
    Task<int> CreateProductAsync(ProductEntity user);
    Task<int> UpdateProductAsync(int id, ProductEntity user);
    Task<int> DeleteProductAsync(int id);
}