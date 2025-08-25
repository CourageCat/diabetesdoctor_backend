using UserService.Contract.Helpers;

namespace UserService.Domain.Events;

public record HospitalCreatedDomainEvent(Guid Id, HospitalProfile HospitalProfile) : IDomainEvent
{
    public static HospitalCreatedDomainEvent Create(HospitalProfile hospitalInfo)
    {
        var id = new UuidV7().Value;
        return new HospitalCreatedDomainEvent(id, hospitalInfo);
    }
}