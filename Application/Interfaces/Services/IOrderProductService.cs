using System.Data;
using Domain;

namespace Application.Interfaces.Services;

public interface IOrderProductService
{
    Task<int> InsertOrderProductAsync(ProductOrderEntity entity, IDbTransaction transaction = null!);
}