using Microsoft.Extensions.Logging;
using UserService.Contract.Infrastructure;

namespace UserService.Application.UseCases.V1.Events;

public sealed class HealthRecordChangedDomainEventHandler
    : IDomainEventHandler<HealthRecordCreatedDomainEvent>
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HealthRecordChangedDomainEventHandler> _logger;

    public HealthRecordChangedDomainEventHandler(
        IBackgroundTaskQueue taskQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<HealthRecordChangedDomainEventHandler> logger)
    {
        _taskQueue = taskQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task Handle(HealthRecordCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        var healthRecord = @event.Record;

        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            using var scope = _scopeFactory.CreateScope();

            try
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var healthRecordRepo = scope.ServiceProvider.GetRequiredService<IRepositoryBase<HealthRecord, Guid>>();
                var carePlanInstanceRepo = scope.ServiceProvider
                    .GetRequiredService<IRepositoryBase<CarePlanMeasurementInstance, Guid>>();
                await HandleUpdateCarePlaneMark(healthRecord, @event.CarePlanInstanceId, unitOfWork, healthRecordRepo,
                    carePlanInstanceRepo, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update careplan");
            }
        });

        return Task.CompletedTask;
    }

    private async Task HandleUpdateCarePlaneMark(
        HealthRecord healthRecord,
        Guid carePlanInstanceId,
        IUnitOfWork unitOfWork,
        IRepositoryBase<HealthRecord, Guid> healthRecordRepo,
        IRepositoryBase<CarePlanMeasurementInstance, Guid> carePlanRepo,
        CancellationToken cancellationToken)
    {
        var existingHealthRecord =
            await healthRecordRepo.FindSingleAsync(p => p.Id == healthRecord.Id, true, cancellationToken);

        if (existingHealthRecord == null)
            throw new Exception("Health Record not existing");
        
        var measuredAtUtc = healthRecord.MeasuredAt;

        var from = measuredAtUtc.AddMinutes(-30);
        var to = measuredAtUtc.AddMinutes(30);

        var carePlanInstance = await carePlanRepo.FindSingleAsync(x =>
            x.Id == carePlanInstanceId &&
            x.PatientProfileId == healthRecord.PatientProfileId &&
            x.RecordType == healthRecord.RecordType &&
            !x.IsCompleted, true, cancellationToken);

        if (carePlanInstance != null && carePlanInstance.ScheduledAt >= from && carePlanInstance.ScheduledAt <= to)
        {
            carePlanInstance.MarkCompleted(healthRecord.MeasuredAt, healthRecord.Id);
            carePlanRepo.Update(carePlanInstance);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}