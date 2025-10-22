using Application.Interfaces.Services;
using Domain;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto.ProductDto;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductController> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    
    public ProductController(IProductService service , ILogger<ProductController> logger,  IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all products");
        var products = await _service.GetAllProductsAsync(cancellationToken);
        if (!products.Any())
        {
            _logger.LogInformation("No products found");
            return NotFound("Products not found");
        }
            
        return Ok(products);
    }
    
    [Authorize(Roles = "User, Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product with id {id}", id);
        var user = await _service.GetProductByIdAsync(id,  cancellationToken);
        if (user == null)
        {
            _logger.LogInformation("No product found");
            return NotFound("Product not found");
        }
            
        return Ok(user);
    }

    [Authorize(Roles = "User, Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductCreationDto  product,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new product");
        var productId  = await _service.CreateProductAsync(product, cancellationToken);
        if (productId < 1)
        {
            _logger.LogInformation("Product creation failed");
            return BadRequest("Unable to create product");
        }

        await _publishEndpoint.Publish(new ProductCreatedEvent(productId, product.Name, product.Price), cancellationToken);
        return Ok($"Product {product.Name} created and published");
    }

    [Authorize(Roles = "Admin")] 
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto product,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product");
        var rowsAffected = await _service.UpdateProductAsync(id, product,  cancellationToken);
        if (rowsAffected == 1)
            return Ok("Product updated successfully");
        
        _logger.LogInformation("Product update failed");
        return BadRequest("Unable to update product");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product");
        var rowsAffected = await _service.DeleteProductAsync(id, cancellationToken);
        if (rowsAffected == 1)
            return Ok("Product deleted  successfully");
        
        _logger.LogWarning("Product deletion failed");
        return BadRequest("Unable to delete product");
        
        
    }

    

}