using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.DTOs.ValueObjectDtos;

namespace ConsultationService.Contract.Services.User.Commands;

public record CreateUserCommand : ICommand
{
    public string Id { get; init; } = null!;
    public FullNameDto FullName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? HospitalId { get; init; }
    public int Role { get; init; }
}