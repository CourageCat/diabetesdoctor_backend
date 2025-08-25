namespace UserService.Domain.Events;

public record HealthRecordCreatedDomainEvent(Guid Id, HealthRecord Record, Guid CarePlanInstanceId) : IDomainEvent
{
    public static HealthRecordCreatedDomainEvent Create(HealthRecord record, Guid carePlanInstanceId, Guid? id = null)
    {
        var actualId = (id is null || id == Guid.Empty) ? Guid.NewGuid() : id.Value;
        return new HealthRecordCreatedDomainEvent(actualId, record, carePlanInstanceId);
    }
}
