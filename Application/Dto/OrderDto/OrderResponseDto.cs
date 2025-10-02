using Application.Dto.ProductDto;

namespace Application.Dto.OrderDto;

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<OrderProductsDto.OrderProductsDto> Products { get; set; } = new List<OrderProductsDto.OrderProductsDto>();
}