namespace Domain;

public class OrderEntity
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    
    //Navigational properties
    public int UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public ICollection<ProductOrderEntity> Items { get; set; } =  new List<ProductOrderEntity>();
    
}