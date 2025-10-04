using Application.Dto.OrderDto;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderProductRepository _orderProductRepository;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IConnectionFactory factory, IOrderProductRepository orderProductRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _connectionFactory = factory;
        _mapper = mapper;
        _orderProductRepository = orderProductRepository;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersWithProductsAsync()
    {
        var orders = await _orderRepository.GetAllOrdersWithProductsAsync();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetOrderWithProductsAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithProductsAsync(id);
        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<int> CreateOrderAsync(OrderCreationDto orderDto)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Маппим DTO → Entity
            var orderEntity = _mapper.Map<OrderEntity>(orderDto);
            orderEntity.CreatedAt = orderDto.OrderDate;

            // 2. Вставляем заказ и получаем его Id
            var orderId = await _orderRepository.InsertOrderAsync(orderEntity, transaction);
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

                await _orderProductRepository.InsertOrderProductAsync(orderProductEntity, transaction);
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
    public Task<int> CancelOrderAsync(int id)
    {
        throw new NotImplementedException();
    }
}