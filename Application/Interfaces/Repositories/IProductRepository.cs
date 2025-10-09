using Domain;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<ProductEntity?>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductEntity?>  GetProductByIdAsync(int id,  CancellationToken cancellationToken = default);
    Task<int> CreateProductAsync(ProductEntity user,  CancellationToken cancellationToken = default);
    Task<int> UpdateProductAsync(int id, ProductEntity user,   CancellationToken cancellationToken = default);
    Task<int> DeleteProductAsync(int id,  CancellationToken cancellationToken = default);
}