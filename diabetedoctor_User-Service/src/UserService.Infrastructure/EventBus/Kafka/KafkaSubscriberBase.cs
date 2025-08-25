using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserService.Application.Helper;
using UserService.Contract.Exceptions;
using UserService.Domain.Abstractions;
using UserService.Domain.Abstractions.Repositories;
using UserService.Domain.Models;
using UserService.Infrastructure.Options;

namespace UserService.Infrastructure.EventBus.Kafka;

public class KafkaSubscriberBase<T> : BackgroundService
{
    private readonly IConsumer<string, EventEnvelope> _consumer;
    private readonly ConsumerWorkerOptions _options;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IIntegrationEventFactory _integrationEventFactory;
    private readonly ILogger<KafkaSubscriberBase<T>> _logger;
    private readonly KafkaSettings _kafkaSettings;
    
    public KafkaSubscriberBase(
        IConsumer<string, EventEnvelope> consumer,
        ConsumerWorkerOptions options,
        ILogger<KafkaSubscriberBase<T>> logger, 
        IServiceScopeFactory serviceScopeFactory, 
        IIntegrationEventFactory integrationEventFactory, KafkaSettings kafkaSettings)
    {
        _consumer = consumer;
        _options = options;
        _serviceScopeFactory = serviceScopeFactory;
        _integrationEventFactory = integrationEventFactory;
        _logger = logger;
        _kafkaSettings = kafkaSettings;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Subscribing to topics [{topic}]...", _options.Topic);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _consumer.Subscribe(_options.Topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(100);

                        if (consumeResult is null)
                        {
                            _logger.LogInformation("No message found in topic [{topic}].", _options.Topic);
                            await Task.Delay(1000, stoppingToken);
                            continue;
                        }
                        await ProcessMessageAsync(consumeResult.Message.Value, stoppingToken);
                        _consumer.Commit(consumeResult);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Consumer error: {ErrorMessage}", ex.Error.Reason);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogInformation("Kafka Background Service Topic [{topic}] has stopped.", _options.Topic);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error consuming message");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to topics");
            }
            
            await Task.Delay(3000, stoppingToken);
        }
        _consumer.Unsubscribe();
        _consumer.Close();
    }

    private async Task ProcessMessageAsync(EventEnvelope messageValue, CancellationToken stoppingToken)
    {
        var retries = 0;
        var @event = _integrationEventFactory.CreateEvent(messageValue.EventTypeName, messageValue.Message);
        if (@event is not null)
        {
            if (_options.AcceptEvent(@event))
            {
                while (retries <= _options.ServiceRetries)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    try
                    {
                        _logger.LogInformation("Processing message {t}: {message}", messageValue.EventTypeName, messageValue.Message);
                        var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(@event.GetType());
                        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                        await ((dynamic)handler).Handle((dynamic)@event, stoppingToken);
                        return;
                    }
                    catch (DomainException ex)
                    {
                        var outboxRepo = scope.ServiceProvider.GetRequiredService<IRepositoryBase<OutboxEvent, Guid>>();
                        var dlqEvent = OutboxEventExtension.ToOutboxEvent(_kafkaSettings.DeadTopic, @event);
                        outboxRepo.Add(dlqEvent);
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                        _logger.LogWarning("Moved to DLQ: {msg}", ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        retries++;
                        _logger.LogError(ex, "Error retry {retries}/{max}", retries, _options.MaxRetries);

                        if (retries > _options.ServiceRetries)
                        {
                            var outboxRepo = scope.ServiceProvider.GetRequiredService<IRepositoryBase<OutboxEvent, Guid>>();

                            if (messageValue.RetryCount > _options.MaxRetries)
                            {
                                var dlqEvent = OutboxEventExtension.ToOutboxEvent(_kafkaSettings.DeadTopic, @event, messageValue.RetryCount);
                                outboxRepo.Add(dlqEvent);
                                await unitOfWork.SaveChangesAsync(stoppingToken);
                                _logger.LogWarning("Moved to DLQ: {msg}", ex.Message);
                                return;
                            }
                            var retryEvent = OutboxEventExtension.ToOutboxEvent(_kafkaSettings.RetryTopic, @event, messageValue.RetryCount + 1);
                            outboxRepo.Add(retryEvent);
                            await unitOfWork.SaveChangesAsync(stoppingToken);
                            _logger.LogWarning("Moved to Retry topic");
                            return;
                        }

                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retries)), stoppingToken);
                    }
                }
            }
            else
            {
                _logger.LogDebug("Event skipped: {t}", messageValue.EventTypeName);
            }
        }
    }
}