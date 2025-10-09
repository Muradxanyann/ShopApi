using Application.Dto.OrderDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/Orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    
    public async Task<IActionResult> GetOrdersWithProductsAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllOrdersWithProductsAsync();
        if (!orders.Any())
            return NotFound("No orders found");
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order =  await _orderService.GetOrderWithProductsAsync(id);
        if (order.OrderId == 0)
            return NotFound("Order not found");
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(OrderCreationDto order)
    {
        var orderId = await _orderService.CreateOrderAsync(order);
        
        return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { orderId });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrderAsync([FromQuery]int id)
    {
        var deleted =  await _orderService.CancelOrderAsync(id);
        if (!deleted)
            return NotFound("Order not found");
        return Ok("Order deleted");
    }
    
}