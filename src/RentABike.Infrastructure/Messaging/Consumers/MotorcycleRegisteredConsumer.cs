using MassTransit;
using RentABike.Domain.Entities;
using RentABike.Domain.Events;
using RentABike.Domain.Interfaces;
using Serilog;

namespace RentABike.Infrastructure.Messaging.Consumers;

public class MotorcycleRegisteredConsumer : IConsumer<MotorcycleRegisteredEvent>
{
    private readonly IMotorcycleNotificationRepository _notificationRepository;
    private readonly ILogger _logger = Log.ForContext<MotorcycleRegisteredConsumer>();

    public MotorcycleRegisteredConsumer(IMotorcycleNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Consume(ConsumeContext<MotorcycleRegisteredEvent> context)
    {
        try
        {
            var eventData = context.Message;

            if (eventData.Year == 2024)
            {
                var notification = new MotorcycleNotification(
                    eventData.MotorcycleId,
                    eventData.Year,
                    eventData.Model,
                    eventData.LicensePlate
                );

                await _notificationRepository.AddAsync(notification);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao processar mensagem de moto cadastrada");
            throw;
        }
    }
}

