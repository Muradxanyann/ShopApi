using Domain;

namespace Shared.Dto.OrderDto;

public class OrderWithUserDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }

    // Минимальные данные пользователя, получаемые через AuthService
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<ProductOrderEntity> Items { get; set; } = new List<ProductOrderEntity>();
}