using System.Data;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain;

namespace Application.Services;

public class OrderProductService : IOrderProductService
{
    private readonly IOrderProductRepository _orderProductRepository;

    public OrderProductService(IOrderProductRepository orderProductRepository)
    {
        _orderProductRepository = orderProductRepository;
    }

    public async Task<int> InsertOrderProductAsync(ProductOrderEntity entity, IDbTransaction transaction = null!)
    {
        return await _orderProductRepository.InsertOrderProductAsync(entity, transaction);
    }
}