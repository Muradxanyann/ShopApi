using System.Text.Json;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;
using Microsoft.Extensions.Logging;
using Shared.Dto.OrderDto;
using Shared.Dto.ProductDto;
using StackExchange.Redis;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly IDatabase _redisDatabase;

    public ProductService(
        IProductRepository productRepository, 
        IMapper mapper,
        ILogger<ProductService> logger, IDatabase redisDatabase
        )
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _redisDatabase = redisDatabase;
    }

    public async Task<IEnumerable<ProductResponseDto?>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        var  products = await _productRepository.GetAllProductsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductResponseDto?>>(products);
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id,  CancellationToken cancellationToken)
    {
        //Using redis for caching product
        var cacheKey = $"product:{id}";
        var cachedProductJson = await _redisDatabase.StringGetAsync(cacheKey);
        if (!cachedProductJson.IsNullOrEmpty)
        {
            _logger.LogInformation("Product {Id} found in cache.", id);
            return JsonSerializer.Deserialize<ProductResponseDto>(cachedProductJson!)!;
        }
        
        _logger.LogInformation("Product  {Id} not found in cache. Fetching from database.", id);
        var productFromDb = await _productRepository.GetProductByIdAsync(id,  cancellationToken);
        if (productFromDb != null)
        {
            var productToCacheJson = JsonSerializer.Serialize(productFromDb);

            await _redisDatabase.StringSetAsync(cacheKey, productToCacheJson, TimeSpan.FromMinutes(10));
            _logger.LogInformation("Product {Id} saved to cache.", id);
        }
        return _mapper.Map<ProductResponseDto?>(productFromDb);
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