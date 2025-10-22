namespace Domain;

public record OrderCreatedEvent(int OrderId, int ProductId, int Quantity);
