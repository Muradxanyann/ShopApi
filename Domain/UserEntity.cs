namespace Domain;

public class UserEntity
{
    public int UserId { get; set; }
    public required string Username { get; set; }     
    public required string Email { get; set; }         
    public required string PasswordHash { get; set; }  
    public required string Role { get; set; } = "User";

    public required string Name { get; set; } 
    public int Age { get; set; }   
    public required string Phone { get; set; }

    // Навигационные свойства // 
    public ICollection<OrderEntity>? Orders { get; set; } = new List<OrderEntity>();
}