
using Shared.Dto.OrderProductsDto;

namespace Shared.Dto.OrderDto;

public class OrderResponseDto
{
    public int OrderId { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public ICollection<OrderProductsInfo> Products { get; init; } = new List<OrderProductsInfo>();
}