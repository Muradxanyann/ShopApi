using Infrastructure.EventConsumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class MassTransitConfiguration
{
    public static void AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            // Регистрируем наш обработчик (consumer)
            busConfigurator.AddConsumer<OrderCreatedConsumer>();
            
            // 🆕 ДОБАВЛЕНО: Регистрируем новый обработчик для событий о продуктах
            busConfigurator.AddConsumer<ProductCreatedConsumer>();

            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var connectionString = configuration["MessageBroker:ConnectionString"];
                rabbitConfigurator.Host(new Uri(connectionString!), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Автоматически настраиваем эндпоинты для ВСЕХ зарегистрированных консьюмеров
                rabbitConfigurator.ConfigureEndpoints(context);
            });
        });
    }
}
