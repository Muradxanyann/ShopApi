namespace Shared.Dto.ProductDto;

public class ProductUpdateDto
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public double Price { get; set; }
}