namespace Domain;

public class ProductEntity
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public double Price { get; set; }
    
    //Navigational properties 
    public ICollection<OrderEntity>? Orders { get; set; } =  new List<OrderEntity>();
}