using Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventConsumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        _logger.LogInformation(
            "RECEIVED ORDER EVENT: Order {OrderId} with ProductId  '{ProductId}' in quantity {Quantity}",
            order.OrderId,
            order.ProductId,
            order.Quantity
        );
        
        // Здесь может быть логика отправки email, обновления статистики и т.д.
        return Task.CompletedTask;
    }
}