using System.Data;
using Domain;

namespace Application.Interfaces.Repositories;

public interface IOrderRepository
{
    public Task<IEnumerable<OrderEntity>> GetAllOrdersWithProductsAsync();
    public Task<OrderEntity?> GetOrderWithProductsAsync(int id);
    public Task<int> InsertOrderAsync(OrderEntity? order, IDbTransaction transaction = null!);
    public Task<int> CancelOrderAsync(int id);
    
}