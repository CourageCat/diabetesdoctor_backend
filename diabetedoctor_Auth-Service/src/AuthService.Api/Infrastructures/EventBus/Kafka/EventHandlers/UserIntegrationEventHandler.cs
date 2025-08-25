using AuthService.Api.Features.Auth.Commands;
using AuthService.Api.Features.Hospital.Commands;
using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;
using AuthService.Api.Infrastructures.EventBus.Events;

namespace AuthService.Api.Infrastructures.EventBus.Kafka.EventHandlers;

public class UserIntegrationEventHandler(ISender sender)
    : IIntegrationEventHandler<UserInfoCreatedProfileIntegrationEvent>, IIntegrationEventHandler<UserInfoUpdatedProfileIntegrationEvent>
{
    public async Task Handle(UserInfoCreatedProfileIntegrationEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Role == (int)RoleType.Patient)
        {
            var fullName = string.Join(" ",
                new[] { @event.FullName.LastName, @event.FullName.MiddleName, @event.FullName.FirstName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            await sender.Send(new PatientCreatedProfileCommand(@event.UserId, fullName, @event.Avatar),
                cancellationToken);
        }
        else if (@event.Role == (int)RoleType.Doctor)
        {
            var fullName = string.Join(" ",
                new[] { @event.FullName.LastName, @event.FullName.MiddleName, @event.FullName.FirstName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            await sender.Send(
                new DoctorCreatedProfileCommand { UserId = @event.UserId, FullName = fullName, Avatar = @event.Avatar, PhoneNumber = @event.PhoneNumber! },
                cancellationToken);
        }
        else if (@event.Role == (int)RoleType.HospitalStaff)
        {
            var fullName = string.Join(" ",
                new[] { @event.FullName.LastName, @event.FullName.MiddleName, @event.FullName.FirstName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            await sender.Send(
                new HospitalStaffCreatedProfileCommand { UserId = @event.UserId, FullName = fullName, Avatar = @event.Avatar, Email = @event.Email! },
                cancellationToken);
        }
        else if (@event.Role == (int)RoleType.HospitalAdmin)
        {
            var fullName = string.Join(" ",
                new[] { @event.FullName.LastName, @event.FullName.MiddleName, @event.FullName.FirstName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            await sender.Send(
                new HospitalAdminCreatedProfileCommand { UserId = @event.UserId, FullName = fullName, Avatar = @event.Avatar, Email = @event.Email! },
                cancellationToken);
        }
    }

    public async Task Handle(UserInfoUpdatedProfileIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var fullName = notification.FullName is not null ? string.Join(" ",
            new[] { notification.FullName.LastName, notification.FullName.MiddleName, notification.FullName.FirstName }
                .Where(x => !string.IsNullOrWhiteSpace(x))) : null;
        await sender.Send(new UpdateProfileCommand{UserId = notification.UserId, FullName = fullName, Avatar = notification.Avatar}, cancellationToken);
    }
}