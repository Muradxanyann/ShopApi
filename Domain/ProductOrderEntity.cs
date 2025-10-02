namespace Domain;

public class ProductOrderEntity
{
    public int OrderId { get; set; }
    public required OrderEntity Order { get; set; }

    public int ProductId { get; set; }
    public required ProductEntity Product { get; set; }

    public int Quantity { get; set; }     
   
}