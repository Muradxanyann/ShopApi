using Application.Dto.ProductDto;

namespace Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto?>> GetAllProductsAsync();
    Task<ProductResponseDto?>  GetProductByIdAsync(int id);
    Task<int> CreateProductAsync(ProductCreationDto user);
    Task<int> UpdateProductAsync(int id, ProductUpdateDto user);
    Task<int> DeleteProductAsync(int id);
}