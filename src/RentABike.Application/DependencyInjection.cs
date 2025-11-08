using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RentABike.Application.Mappings;
using RentABike.Application.Services;
using RentABike.Application.Services.Interfaces;

namespace RentABike.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IMotorcycleService, MotorcycleService>();
        services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();
        services.AddScoped<IRentalService, RentalService>();

        // AutoMapper
        services.AddAutoMapper(typeof(Mappings.MappingProfile));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
