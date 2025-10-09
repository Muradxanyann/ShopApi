
using Application.Dto.ProductDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductController> _logger;
    
    public ProductController(IProductService service , ILogger<ProductController> logger)
    {
        _service = service;
        _logger = logger;
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

    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductCreationDto  product,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new product");
        var rowsAffected = await _service.CreateProductAsync(product, cancellationToken);
        if (rowsAffected == 1)
            return Ok("Product created successfully");
        
        _logger.LogInformation("Product creation failed");
        return BadRequest("Unable to create product");
    }

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product");
        var rowsAffected = await _service.DeleteProductAsync(id, cancellationToken);
        if (rowsAffected == 1)
            return Ok("Product deleted  successfully");
        
        _logger.LogInformation("Product deletion failed");
        return BadRequest("Unable to delete product");
    }
}