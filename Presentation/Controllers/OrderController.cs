using Application.Dto.OrderDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;
    

    public OrderController(IOrderService orderService,  ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

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

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(OrderCreationDto order,  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new order");
        var orderId = await _orderService.CreateOrderAsync(order, cancellationToken);
        if (orderId == 0)
        {
            _logger.LogInformation("No order with id {id} found", orderId);
            return NotFound("Order not found");
        }
        
        return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { orderId });
    }

    [HttpDelete]
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