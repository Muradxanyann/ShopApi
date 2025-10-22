using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces.Services;
using Domain;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Dto.OrderDto;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;
    private readonly UserClient _client;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public OrderController(
        IOrderService orderService,
        ILogger<OrderController> logger,
        UserClient client,
        IPublishEndpoint publishEndpoint)
    {
        _orderService = orderService;
        _logger = logger;
        _client = client;
        _publishEndpoint = publishEndpoint;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetOrdersWithProductsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting orders with products");
        var orders = await _orderService.GetAllOrdersWithProductsAsync(cancellationToken);
        if (!orders.Any())
        {
            _logger.LogInformation("No orders found");
            return NotFound("No orders found");
        }
            
        return Ok(orders);
    }
   
    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting order with id {id}", id);
        var order =  await _orderService.GetOrderWithProductsAsync(id,  cancellationToken);
        if (order.OrderId == 0)
        {
            _logger.LogInformation("No order with id {id} found", id);
            return NotFound("Order not found");
        }
            
        return Ok(order);
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(OrderCreationDto order,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new order");
        var orderId = await _orderService.CreateOrderAsync(order, cancellationToken);
        switch (orderId)
        {
            case 0:
                _logger.LogInformation("No order with id {id} found", orderId);
                return NotFound("Order not found");
            case -1:
                _logger.LogInformation("No user with id {id} found", order.UserId);
                return NotFound("User not found");
        }

        await _publishEndpoint.Publish(
            new OrderCreatedEvent(orderId, order.OrderProducts.First().ProductId, order.OrderProducts.Count),
            cancellationToken);
        return Ok("Order created and published");
    }

    [HttpDelete]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> DeleteOrderAsync([FromQuery]int id,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting order with id {id}", id);
        var deleted =  await _orderService.CancelOrderAsync(id, cancellationToken);
        if (!deleted)
        {
            _logger.LogInformation("Cannot delete order");
            return NotFound("Order not found");
        }
        
        return Ok("Order deleted");
    }
    
}