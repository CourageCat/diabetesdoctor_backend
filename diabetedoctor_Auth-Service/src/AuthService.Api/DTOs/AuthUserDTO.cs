namespace AuthService.Api.DTOs;

public record AuthUserDto(
  string? Id = null,
  string? FullName = null,
  string? AvatarUrl = null,
  bool? IsFirstUpdated = null,
  List<string>? Roles = null
);
