using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using Domain;
using Moq;
using Shared.Dto.ProductDto;
using Xunit.Abstractions;

namespace Tests.Services;

public class ProductServiceTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _mockService;
    public ProductServiceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockService = new ProductService(_mockProductRepository.Object, _mockMapper.Object);
    }

    private static List<ProductEntity> CreateFakeProducts() =>
    [
        new() { ProductId = 1, Category = "Electronics", Name = "Phone" },
        new() { ProductId = 2, Category = "Electronics", Name = "TV" }
    ];
    
    private static List<ProductResponseDto> CreateFakeProductsDto() =>
    [
        new() { ProductId = 1, Category = "Electronics", Name = "Phone" },
        new() { ProductId = 2, Category = "Electronics", Name = "TV" }
    ];
    

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAll_MappedProducts()
    {
        var cancellationToken = CancellationToken.None;
        var productList = CreateFakeProducts();

        var mappedProducts = CreateFakeProductsDto();
        
        _mockProductRepository
            .Setup(repo => repo.GetAllProductsAsync(cancellationToken))
            .ReturnsAsync(productList);
        
        _mockMapper
            .Setup(m => m.Map<IEnumerable<ProductResponseDto>>(productList))
            .Returns(mappedProducts);
        
        var result = await _mockService.GetAllProductsAsync(cancellationToken);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        
        _mockProductRepository.Verify(repo => repo.GetAllProductsAsync(cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<ProductResponseDto>>(productList), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturn_MappedProduct()
    {
        var cancellationToken = CancellationToken.None;
        
        var products = CreateFakeProducts();
        var mappedProducts = CreateFakeProductsDto();
        
        _mockProductRepository
            .Setup(repo => repo.GetProductByIdAsync(products[0].ProductId, cancellationToken))
            .ReturnsAsync(products[0]);
        
        _mockMapper.Setup(m => m.Map<ProductResponseDto>(products[0]))
            .Returns(mappedProducts[0]);
        
        var result = await _mockService.GetProductByIdAsync(products[0].ProductId, cancellationToken);
        Assert.NotNull(result);
        Assert.Equal(products[0].ProductId, result.ProductId);
        Assert.Equal(mappedProducts[0], result);
        
        _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(products[0].ProductId, cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<ProductResponseDto>(products[0]), Times.Once);
        
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var cancellationToken = CancellationToken.None;

        _mockProductRepository
            .Setup(repo => repo.GetProductByIdAsync(It.IsAny<int>(), cancellationToken))
            .ReturnsAsync((ProductEntity)null!);
        
        var result = await _mockService.GetProductByIdAsync(It.IsAny<int>(), cancellationToken);

        Assert.Null(result);
        _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(It.IsAny<int>(), cancellationToken), Times.Once);
        
    }
    
}