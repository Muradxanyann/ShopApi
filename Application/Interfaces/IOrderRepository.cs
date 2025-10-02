using Application.Dto.OrderDto;

namespace Application.Interfaces;

public interface IOrderRepository
{
    public Task<IEnumerable<OrderResponseDto?>> GetAllOrdersWithProductsAsync();
}