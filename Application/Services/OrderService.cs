using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;
using Shared;
using Shared.Dto.OrderDto;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderProductRepository _orderProductRepository;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IMapper _mapper;
    private readonly UserClient  _client;

    public OrderService(IOrderRepository orderRepository, IConnectionFactory factory, 
        IOrderProductRepository orderProductRepository, IMapper mapper,  UserClient client)
    {
        _orderRepository = orderRepository;
        _connectionFactory = factory;
        _mapper = mapper;
        _orderProductRepository = orderProductRepository;
        _client = client;
    }
    
    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersWithProductsAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllOrdersWithProductsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetOrderWithProductsAsync(int id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderWithProductsAsync(id,  cancellationToken);
        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<int> CreateOrderAsync(OrderCreationDto orderDto, CancellationToken cancellationToken)
    {
        var user =  await  _client.GetUserAsync(orderDto.UserId, cancellationToken);
        
        if (user == null)
        {
            return -1;
        }
        
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            // 1. Маппим DTO → Entity
            var orderEntity = _mapper.Map<OrderEntity>(orderDto);
            orderEntity.UserId = user.Id;

            // 2. Вставляем заказ и получаем его Id
            var orderId = await _orderRepository.InsertOrderAsync(orderEntity, transaction, cancellationToken);
            if (orderId == 0)
                throw new Exception("Order could not be created");
            

            // 3. Вставляем продукты заказа
            foreach (var productDto in orderDto.OrderProducts)
            {
                var orderProductEntity = new ProductOrderEntity()
                {
                    OrderId = orderId,
                    ProductId = productDto.ProductId,
                    Quantity = productDto.Quantity
                };

                await _orderProductRepository.InsertOrderProductAsync(orderProductEntity, transaction, cancellationToken);
            }

            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    public async Task<bool> CancelOrderAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var transaction = connection.BeginTransaction();
        try
        {
           var deletedOrderProduct = await _orderProductRepository.DeleteOrderProductAsync(id, transaction, cancellationToken);
           if (deletedOrderProduct == 0)
               return false;
           
           var deletedOrder = await _orderRepository.CancelOrderAsync(id, transaction, cancellationToken);
           if (deletedOrder == 0)
               return false;
           transaction.Commit();
           return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    
}