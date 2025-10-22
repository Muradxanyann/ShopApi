using Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventConsumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(ILogger<ProductCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var product = context.Message;
        
        _logger.LogInformation(
            "RECEIVED PRODUCT EVENT (Notification to users): New product released! {ProductName} (ID: {ProductId}) in price {Price}",
            product.ProductName,
            product.ProductId,
            product.Price
        );
        
        return Task.CompletedTask;
    }
}