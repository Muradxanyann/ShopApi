using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("Admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult Get()
    {
        return Ok("Answer from Admin");
    }
    
    [HttpGet("User")]
    [Authorize(Roles = "User")]
    public IActionResult GetUser()
    {
        return Ok("Answer from user");
    }
}