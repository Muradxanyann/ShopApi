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
    
    public async Task<IActionResult> GetOrdersWithProductsAsync()
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
        if (order == null)
            return NotFound("Order not found");
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(OrderCreationDto order)
    {
        var orderId = await _orderService.CreateOrderAsync(order);
        
        return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { orderId });
    }
}