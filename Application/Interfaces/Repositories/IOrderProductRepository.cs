using System.Data;
using Domain;

namespace Application.Interfaces.Repositories;

public interface IOrderProductRepository
{
    Task<int> InsertOrderProductAsync(ProductOrderEntity entity, IDbTransaction transaction =  null!);
    Task<int> DeleteOrderProductAsync(int orderId, IDbTransaction transaction = null!);
}