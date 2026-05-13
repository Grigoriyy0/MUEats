using Microsoft.Extensions.Hosting;

namespace MUEats.Restaurants.Infrastructure.Workers;

public class OutboxProcessingWorker : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}