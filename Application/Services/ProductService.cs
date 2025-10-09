using Application.Dto.ProductDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductResponseDto?>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        var  products = await _productRepository.GetAllProductsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductResponseDto?>>(products);
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id,  CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(id,  cancellationToken);
        return _mapper.Map<ProductResponseDto?>(product);
    }

    public async Task<int> CreateProductAsync(ProductCreationDto user,  CancellationToken cancellationToken)
    {
        var product = _mapper.Map<ProductEntity>(user);
        return await _productRepository.CreateProductAsync(product,  cancellationToken);
    }

    public async Task<int> UpdateProductAsync(int id, ProductUpdateDto user,  CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ProductEntity>(user);
        return await _productRepository.UpdateProductAsync(id, entity,  cancellationToken);
    }

    public async Task<int> DeleteProductAsync(int id,   CancellationToken cancellationToken)
    {
        return await _productRepository.DeleteProductAsync(id,  cancellationToken);
    }
}