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

    public async Task<IEnumerable<ProductResponseDto?>> GetAllProductsAsync()
    {
        var  products = await _productRepository.GetAllProductsAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto?>>(products);
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        return _mapper.Map<ProductResponseDto?>(product);
    }

    public async Task<int> CreateProductAsync(ProductCreationDto user)
    {
        var product = _mapper.Map<ProductEntity>(user);
        return await _productRepository.CreateProductAsync(product);
    }

    public async Task<int> UpdateProductAsync(int id, ProductUpdateDto user)
    {
        var entity = _mapper.Map<ProductEntity>(user);
        return await _productRepository.UpdateProductAsync(id, entity);
    }

    public async Task<int> DeleteProductAsync(int id)
    {
        return await _productRepository.DeleteProductAsync(id);
    }
}