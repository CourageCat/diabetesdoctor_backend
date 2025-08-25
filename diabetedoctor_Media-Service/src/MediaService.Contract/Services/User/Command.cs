using MediaService.Contract.DTOs.UserDTOs;
using MediaService.Contract.Enumarations;

namespace MediaService.Contract.Services.User;

public record CreateUserCommand(string Id, FullNameDto FullName, string PublicUrl, int Role) : ICommand<Success>;
public record UpdateUserCommand(string UserId, FullNameDto? FullName, string? Avatar) :  ICommand<Success>;
