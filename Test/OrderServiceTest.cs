using System.Data;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using Domain;
using Moq;
using Shared;
using Shared.Dto.OrderDto;
using Shared.Dto.OrderProductsDto;
using Shared.Dto.UserDto;

namespace Tests.Services;

public class OrderServiceTest
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IOrderProductRepository> _mockOrderProductRepository;
    private readonly Mock<IConnectionFactory> _mockConnectionFactory;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<UserClient> _mockUserClient;
    private readonly OrderService _orderService;

    public OrderServiceTest()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockOrderProductRepository = new Mock<IOrderProductRepository>();
        _mockConnectionFactory = new Mock<IConnectionFactory>();
        _mockMapper = new Mock<IMapper>();
        _mockUserClient = new Mock<UserClient>();

        _orderService = new OrderService(
            _mockOrderRepository.Object,
            _mockConnectionFactory.Object,
            _mockOrderProductRepository.Object,
            _mockMapper.Object,
            _mockUserClient.Object
        );
    }

    [Fact]
    public async Task GetAllOrdersWithProductsAsync_ShouldReturnMappedDtos()
    {
        // arrange 
        var cancellationToken = CancellationToken.None;

        var orderEntityFromRepo = new List<OrderEntity>()
        {
            new OrderEntity() { OrderId = 1 },
            new OrderEntity() { OrderId = 2 },
        };

        _mockOrderRepository
            .Setup(m => m.GetAllOrdersWithProductsAsync(cancellationToken))
            .ReturnsAsync(orderEntityFromRepo);

        var mappedOrderResponseDtos = new List<OrderResponseDto>()
        {
            new OrderResponseDto() { OrderId = 1 },
            new OrderResponseDto() { OrderId = 2 },
        };

        _mockMapper
            .Setup(m => m.Map<IEnumerable<OrderResponseDto>>(orderEntityFromRepo))
            .Returns(mappedOrderResponseDtos);

        // act
        var result = await _orderService.GetAllOrdersWithProductsAsync(cancellationToken);

        // assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        _mockOrderRepository.Verify(m => m.GetAllOrdersWithProductsAsync(cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<OrderResponseDto>>(orderEntityFromRepo), Times.Once);
    }


    [Fact]
    public async Task GetAllOrdersWithProductsAsync_ShouldReturnEmpty_WhenNoOrders()
    {
        //arange
        var cancellationToken = CancellationToken.None;
        var emptyOrders = new List<OrderEntity>();

        _mockOrderRepository
            .Setup(m => m.GetAllOrdersWithProductsAsync(cancellationToken))
            .ReturnsAsync(emptyOrders);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<OrderResponseDto>>(emptyOrders))
            .Returns(new List<OrderResponseDto>());

        //act
        var orders = await _orderService.GetAllOrdersWithProductsAsync(cancellationToken);

        //assert
        Assert.Empty(orders);
        Assert.NotNull(orders);

        _mockOrderRepository.Verify(m => m.GetAllOrdersWithProductsAsync(cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<OrderResponseDto>>(emptyOrders), Times.Once);

    }

    [Fact]
    public async Task GetOrderWithProductsAsync_ShouldReturnMappedDtos()
    {
        var cancellationToken = CancellationToken.None;

        var orderFromRepo = new OrderEntity() { OrderId = 1 };
        _mockOrderRepository
            .Setup(m => m.GetOrderWithProductsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderFromRepo);

        var mappedOrderResponseDtos = new OrderResponseDto() { OrderId = 1 };
        _mockMapper.Setup(m => m.Map<OrderResponseDto>(orderFromRepo))
            .Returns(mappedOrderResponseDtos);

        var result = await _orderService.GetOrderWithProductsAsync(1, cancellationToken);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.OrderId);
        _mockOrderRepository.Verify(m => m.GetOrderWithProductsAsync(1, cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<OrderResponseDto>(orderFromRepo), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ShouldReturnOrderId()
    {
        var cancellationToken = CancellationToken.None;
        
        var userFromService = new UserDto()
        {
            Id = 1,
            Name = "John",
            Email = "John@gmail.com"
        };

        var orderCreationDto = new OrderCreationDto()
        {
            UserId = 1,
            OrderProducts = new List<OrderProductsCreationDto>()
            {
                new OrderProductsCreationDto ()
                {
                    ProductId = 1,
                    Quantity = 1
                }
            }
        };
        
        var orderEntity = new OrderEntity() { OrderId = 1 };
        
        var mockConnection = new Mock<IDbConnection>();
        var mockTransaction = new Mock<IDbTransaction>();
       
        mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
        _mockConnectionFactory
            .Setup(f => f.CreateConnection())
            .Returns(mockConnection.Object);
        
        _mockUserClient
            .Setup(u => u.GetUserAsync(userFromService.Id, cancellationToken))
            .ReturnsAsync(userFromService);

        _mockMapper
            .Setup(m => m.Map<OrderEntity>(orderCreationDto))
            .Returns(orderEntity);
        
        _mockOrderRepository
            .Setup(o => o.InsertOrderAsync(It.IsAny<OrderEntity>(), It.IsAny<IDbTransaction>(), cancellationToken))
            .ReturnsAsync(1);

        _mockOrderProductRepository
            .Setup(op => op.InsertOrderProductAsync(It.IsAny<ProductOrderEntity>(), It.IsAny<IDbTransaction>(), cancellationToken))
            .ReturnsAsync(1);
        
        
        //act
       var result = await _orderService.CreateOrderAsync(orderCreationDto, cancellationToken);
        
        //assert
        Assert.NotEqual(-1, result);
        Assert.Equal(1, result);
        
        _mockOrderRepository.Verify(o => o.InsertOrderAsync(orderEntity, It.IsAny<IDbTransaction>(), cancellationToken), Times.Once);
        _mockOrderProductRepository.Verify(op => op.InsertOrderProductAsync(
            It.Is<ProductOrderEntity>(x => x.ProductId == 1 && x.Quantity == 1),
            It.IsAny<IDbTransaction>(),
            cancellationToken), Times.Once);  
        _mockMapper.Verify(m => m.Map<OrderEntity>(orderCreationDto), Times.Once);
        mockTransaction.Verify(t => t.Commit(), Times.Once);
    }
    
    [Fact]
    public async Task CreateOrderAsync_WhenUserNotFound_ShouldReturnNull()
    {
        var cancellationToken = CancellationToken.None;
        var orderCreationDto = new OrderCreationDto { UserId = 999 };

        _mockUserClient
            .Setup(u => u.GetUserAsync(orderCreationDto.UserId, cancellationToken))
            .ReturnsAsync((UserDto?)null);

        var result = await _orderService.CreateOrderAsync(orderCreationDto, cancellationToken);
        
        Assert.Equal(-1, result);
        _mockOrderRepository.Verify(o => o.InsertOrderAsync(It.IsAny<OrderEntity>(), It.IsAny<IDbTransaction>(), cancellationToken),
            Times.Never);
    }
    
    [Fact]
    public async Task CreateOrderAsync_When_InsertOrderFails_ShouldRollbackTransaction()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var user = new UserDto { Id = 1, Name = "John", Email = "john@gmail.com" };
        var orderDto = new OrderCreationDto { UserId = 1 };

        var mockConnection = new Mock<IDbConnection>();
        var mockTransaction = new Mock<IDbTransaction>();

        mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
        _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

        _mockUserClient.Setup(u => u.GetUserAsync(user.Id, cancellationToken)).ReturnsAsync(user);

        _mockOrderRepository
            .Setup(o => o.InsertOrderAsync(It.IsAny<OrderEntity>(), It.IsAny<IDbTransaction>(), cancellationToken))
            .ThrowsAsync(new Exception("DB insert failed"));

        
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _orderService.CreateOrderAsync(orderDto, cancellationToken));
        
        mockTransaction.Verify(t => t.Rollback(), Times.Once);
        mockTransaction.Verify(t => t.Commit(), Times.Never);
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldCommitTransaction_WhenEverythingIsOk()
    {
        var cancellationToken = CancellationToken.None;

        var order = new OrderEntity() { OrderId = 1 };
        var mockConnection = new Mock<IDbConnection>();
        var mockTransaction = new Mock<IDbTransaction>();
        mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
        _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

        _mockOrderProductRepository
            .Setup(op => op.DeleteOrderProductAsync(order.OrderId, It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        _mockOrderRepository
            .Setup(o => o.CancelOrderAsync(order.OrderId, It.IsAny<IDbTransaction>(), cancellationToken))
            .ReturnsAsync(1);
        
        var result = await _orderService.CancelOrderAsync(order.OrderId, cancellationToken);
        
        Assert.True(result);
        
        _mockOrderProductRepository.Verify(op => 
            op.DeleteOrderProductAsync(order.OrderId, It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>()), Times.Once);

        _mockOrderRepository.Verify(o => 
            o.CancelOrderAsync(order.OrderId, It.IsAny<IDbTransaction>(), cancellationToken), Times.Once);
        
        mockTransaction.Verify(t => t.Commit(), Times.Once);
        mockTransaction.Verify(t => t.Rollback(), Times.Never);
    }
    
    [Fact]
    public async Task CancelOrderAsync_WhenOrderIdIsNotValid_ShouldReturnFalse()
    {
        var cancellationToken = CancellationToken.None;
        var order = new OrderEntity() { OrderId = 999 };
        
        var mockConnection = new Mock<IDbConnection>();
        var mockTransaction = new Mock<IDbTransaction>();
        mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
        _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);
        
        _mockOrderProductRepository
            .Setup(op => op.DeleteOrderProductAsync(order.OrderId, It.IsAny<IDbTransaction>(), cancellationToken))
            .ReturnsAsync(0);
        
        var result = await _orderService.CancelOrderAsync(order.OrderId, cancellationToken);
        
        Assert.False(result);
        
        _mockOrderProductRepository
            .Verify(o => o.DeleteOrderProductAsync(order.OrderId,  It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _mockOrderRepository
            .Verify(o => o.CancelOrderAsync(order.OrderId, It.IsAny<IDbTransaction>(), cancellationToken), Times.Never);
        
        mockTransaction.Verify(t => t.Rollback(), Times.Once);
        mockTransaction.Verify(t => t.Commit(), Times.Never);
        
    }
    
    


}
