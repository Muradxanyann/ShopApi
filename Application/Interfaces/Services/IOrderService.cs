using Shared.Dto.OrderDto;

namespace Application.Interfaces.Services;

public interface IOrderService
{
    public Task<IEnumerable<OrderResponseDto>> GetAllOrdersWithProductsAsync(CancellationToken cancellationToken = default);
    public Task<OrderResponseDto> GetOrderWithProductsAsync(int id, CancellationToken cancellationToken = default);
    public Task<int> CreateOrderAsync(OrderCreationDto order,  CancellationToken cancellationToken = default);
    public Task<bool> CancelOrderAsync(int id,  CancellationToken cancellationToken = default);
}