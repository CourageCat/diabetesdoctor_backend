using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsultationService.Infrastructure.Outbox;

internal class OutboxScheduleBackgroundService(
    ILogger<OutboxScheduleBackgroundService> logger,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private const int OutboxScheduleProcessorFrequency = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting outbox background service");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();
                await outboxProcessor.ExecuteSchedule(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(OutboxScheduleProcessorFrequency), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("OutboxBackgroundService cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred in OutboxBackgroundService");
        }
        finally
        {
            logger.LogInformation("Outbox background finished ...");
        }
    }
}