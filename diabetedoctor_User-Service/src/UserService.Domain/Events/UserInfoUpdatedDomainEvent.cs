using UserService.Contract.Helpers;

namespace UserService.Domain.Events;

public record UserInfoUpdatedDomainEvent(Guid Id, Guid UserId, FullName? FullName, string? Avatar) : IDomainEvent
{
    public static UserInfoUpdatedDomainEvent CreateUpdatedProfileEvent(Guid userId, FullName? fullName, string? avatar)
    {
        var id = new UuidV7().Value;
        return new UserInfoUpdatedDomainEvent(id, userId, fullName, avatar);
    }
}