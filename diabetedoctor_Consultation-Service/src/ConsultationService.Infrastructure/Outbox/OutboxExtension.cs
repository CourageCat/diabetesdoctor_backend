using ConsultationService.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsultationService.Infrastructure.Outbox;

public static class OutboxExtension
{
    public static void AddOutboxBackgroundService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHostedService<OutboxBackgroundService>();
        builder.Services.AddHostedService<OutboxScheduleBackgroundService>();
    }
    
    public static void AddOutboxRetryBackgroundService(this IHostApplicationBuilder builder, Action<OutboxOptions> configure)
    {
        var config = new OutboxOptions();
        configure(config);
        builder.Services.AddHostedService(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<OutboxRetryBackgroundService>>();
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            return new OutboxRetryBackgroundService(logger, scopeFactory, config);
        });
    }
}