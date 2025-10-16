using System.Data;
using Domain;
using Shared.Dto.OrderDto;

namespace Application.Interfaces.Repositories;

public interface IOrderRepository
{
    public Task<IEnumerable<OrderEntity>> GetAllOrdersWithProductsAsync(CancellationToken cancellationToken = default);
    public Task<OrderEntity?> GetOrderWithProductsAsync(int id,  CancellationToken cancellationToken = default);
    public Task<int> InsertOrderAsync(OrderEntity? order, IDbTransaction transaction = null!,  CancellationToken cancellationToken = default);
    public Task<int> CancelOrderAsync(int id,  IDbTransaction transaction = null!, CancellationToken cancellationToken = default);
    


}