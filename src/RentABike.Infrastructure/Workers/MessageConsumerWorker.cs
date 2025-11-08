using MassTransit;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace RentABike.Infrastructure.Workers;

public class MessageConsumerWorker : BackgroundService
{
    private readonly IBus _bus;
    private readonly ILogger _logger = Log.ForContext<MessageConsumerWorker>();

    public MessageConsumerWorker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.Information("Worker de consumo de mensagens iniciado");

            _logger.Information("Bus de mensageria configurado e pronto para consumir mensagens");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro no worker de consumo de mensagens");
            throw;
        }
        finally
        {
            _logger.Information("Worker de consumo de mensagens finalizado");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Solicitação de parada do worker recebida");
        await base.StopAsync(cancellationToken);
    }
}

