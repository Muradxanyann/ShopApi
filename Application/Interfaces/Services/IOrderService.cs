using Application.Dto.OrderDto;

namespace Application.Interfaces.Services;

public interface IOrderService
{
    public Task<IEnumerable<OrderResponseDto>> GetAllOrdersWithProductsAsync();
    
    public Task<OrderResponseDto> GetOrderWithProductsAsync(int id);
    public Task<int> CreateOrderAsync(OrderCreationDto order);
    public Task<bool> CancelOrderAsync(int id);
}