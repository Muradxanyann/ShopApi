using Application.Dto.OrderProductsDto;
using Application.Dto.ProductDto;

namespace Application.Dto.OrderDto;

public class OrderResponseDto
{
    public int OrderId { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public ICollection<OrderProductsInfo> Products { get; init; } = new List<OrderProductsInfo>();
}