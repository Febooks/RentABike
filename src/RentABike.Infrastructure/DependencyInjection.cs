using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentABike.Domain.Interfaces;
using RentABike.Infrastructure.Data;
using RentABike.Infrastructure.Messaging;
using RentABike.Infrastructure.Messaging.Consumers;
using RentABike.Infrastructure.Repositories;
using RentABike.Infrastructure.Storage;
using RentABike.Infrastructure.Workers;

namespace RentABike.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IMotorcycleNotificationRepository, MotorcycleNotificationRepository>();

        // Storage
        services.AddScoped<IStorageService, LocalStorageService>();

        // Message Bus
        services.AddMassTransit(x =>
        {
            x.AddConsumer<MotorcycleRegisteredConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IMessageBus, MessageBus>();

        // Worker
        services.AddHostedService<MessageConsumerWorker>();

        return services;
    }
}
