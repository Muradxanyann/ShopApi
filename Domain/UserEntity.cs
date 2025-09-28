namespace Domain;

public class UserEntity
{
    public int  Id { get; set; }
    public required string Name { get; set; } 
    public int Age { get; set; }   
    public required string Phone { get; set; }
    public required string Email { get; set; }
    //Navigational property
    public ICollection<OrderEntity>? Orders { get; set; }
}