namespace Domain;

public class OrderEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Quantity { get; set; }
    
    //Navigational properties
    public int UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public ICollection<ProductEntity>? Products { get; set; }
    
}