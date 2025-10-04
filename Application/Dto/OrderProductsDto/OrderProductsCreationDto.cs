namespace Application.Dto.OrderProductsDto;

public class OrderProductsCreationDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public decimal Price { get; set; }  
}