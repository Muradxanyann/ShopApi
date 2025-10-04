
using Application.Dto.ProductDto;
using Application.Dto.UserDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    
    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _service.GetAllProductsAsync();
        if (!products.Any())
            return NotFound("Products not found");
        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var user = await _service.GetProductByIdAsync(id);
        if (user == null)
            return NotFound("Product not found");
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductCreationDto  product)
    {
        var rowsAffected = await _service.CreateProductAsync(product);
        if (rowsAffected == 1)
            return Ok("Product created successfully");
        
        return BadRequest("Unable to create product");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto product)
    {
        var rowsAffected = await _service.UpdateProductAsync(id, product);
        if (rowsAffected == 1)
            return Ok("Product updated successfully");
        
        return BadRequest("Unable to update product");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var rowsAffected = await _service.DeleteProductAsync(id);
        if (rowsAffected == 1)
            return Ok("Product deleted  successfully");
        
        return BadRequest("Unable to delete product");
    }
}