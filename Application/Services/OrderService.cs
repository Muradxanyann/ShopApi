using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Dto.OrderDto;
using StackExchange.Redis;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderProductRepository _orderProductRepository;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IMapper _mapper;
    private readonly UserClient  _client;
    private readonly ILogger<OrderService> _logger;
    
    private readonly IDatabase _redisDatabase;

    public OrderService(
		IOrderRepository orderRepository,
		IConnectionFactory factory, 
        IOrderProductRepository orderProductRepository,
		IMapper mapper,  
		UserClient client,
        IDatabase redis,
		ILogger<OrderService> logger
        )
    {
        _orderRepository = orderRepository;
        _connectionFactory = factory;
        _mapper = mapper;
        _orderProductRepository = orderProductRepository;
        _client = client;
        _redisDatabase = redis;
        _logger = logger;
    }
    
    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersWithProductsAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllOrdersWithProductsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetOrderWithProductsAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";
        var cachedProductJson = await _redisDatabase.StringGetAsync(cacheKey);
        if (!cachedProductJson.IsNullOrEmpty)
        {
            _logger.LogInformation("Order {Id} found in cache.", id);
            return JsonSerializer.Deserialize<OrderResponseDto>(cachedProductJson!)!;
        }
        
        _logger.LogWarning("Order  {Id} not found in cache. Fetching from database.", id);
        var orderFromDb  = await _orderRepository.GetOrderWithProductsAsync(id,  cancellationToken);
        
        if (orderFromDb != null)
        {
            var productToCacheJson = JsonSerializer.Serialize(orderFromDb);

            await _redisDatabase.StringSetAsync(cacheKey, productToCacheJson, TimeSpan.FromMinutes(10));
            Console.WriteLine($"Order {id} saved to cache.");
        }

        return _mapper.Map<OrderResponseDto>(orderFromDb);
    }

    public async Task<int> CreateOrderAsync(OrderCreationDto orderDto, CancellationToken cancellationToken)
    {
        var user =  await _client.GetUserAsync(orderDto.UserId, cancellationToken);
        
        if (user == null)
        {
            return -1;
        }
        
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            // From DTO → Entity
            var orderEntity = _mapper.Map<OrderEntity>(orderDto);
            orderEntity.UserId = user.Id;

            // 2. Getting order Id
            var orderId = await _orderRepository.InsertOrderAsync(orderEntity, transaction, cancellationToken);
            if (orderId == 0)
                throw new Exception("Order could not be created");
            
            // 3. Main part!!!, inserting products into order
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