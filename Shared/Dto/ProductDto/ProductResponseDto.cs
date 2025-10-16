namespace Shared.Dto.ProductDto;

public class ProductResponseDto
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public decimal Price { get; set; }
}