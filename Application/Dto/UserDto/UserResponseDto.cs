using Application.Dto.OrderDto;

namespace Application.Dto.UserDto;

public class UserResponseDto
{
    public int  UserId { get; set; }
    public required string Name { get; set; } 
    public int Age { get; set; }   
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public ICollection<OrderResponseDto>? Orders { get; set; } = new List<OrderResponseDto>();
}