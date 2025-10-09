using Application.Dto.ProductDto;

namespace Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto?>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductResponseDto?>  GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateProductAsync(ProductCreationDto user, CancellationToken cancellationToken = default);
    Task<int> UpdateProductAsync(int id, ProductUpdateDto user, CancellationToken cancellationToken = default);
    Task<int> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
}