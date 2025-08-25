namespace AuthService.Api.DTOs;

public record UserDto(
  Guid? Id = null,
  string? FullName = null,
  string? PhoneNumber = null,
  string? AvatarUrl = null,
  string? PasswordHash = null,
  bool? IsFirstUpdated = null,
  string? Otp = null,
  List<string>? Roles = null
);
