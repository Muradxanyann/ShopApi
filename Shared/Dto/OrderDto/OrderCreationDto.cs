
using Shared.Dto.OrderProductsDto;

namespace Shared.Dto.OrderDto;

public class OrderCreationDto
{
    public int UserId { get; set; }
    public ICollection<OrderProductsCreationDto>  OrderProducts { get; set; } = new List<OrderProductsCreationDto>();
}