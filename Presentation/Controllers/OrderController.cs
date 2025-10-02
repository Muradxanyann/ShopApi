using Application;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;

    public OrderController(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersWithProductsAsync()
    {
        var orders = await _orderRepository.GetAllOrdersWithProductsAsync();
        if (!orders.Any())
            return NotFound("No orders found");
        return Ok(orders);
    }
}