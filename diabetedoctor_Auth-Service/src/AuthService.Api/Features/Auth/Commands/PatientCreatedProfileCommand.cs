namespace AuthService.Api.Features.Auth.Commands;

public record PatientCreatedProfileCommand
   (Guid UserId, string FullName, string Avatar) : ICommand<Success>;
