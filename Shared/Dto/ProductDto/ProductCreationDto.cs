namespace Application.Dto.ProductDto;

public class ProductCreationDto
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public double Price { get; set; }
}