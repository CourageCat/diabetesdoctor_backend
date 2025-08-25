using AuthService.Api.Common.DomainErrors;
using AuthService.Api.Features.Auth.Responses;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

/// <summary>
/// Xử lý đăng nhập bằng số điện thoại. Kiểm tra người dùng và mật khẩu,
/// tạo JWT và lưu phản hồi đăng nhập vào cache.
/// </summary>
public sealed class LoginWithEmailCommandHandler
    : ICommandHandler<LoginWithEmailCommand, Success<LoginResponse>>
{
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IResponseCacheService _responseCacheService;
    private readonly IJwtProviderService _jwtProviderService;
    private readonly AuthSettings _authSettings;

    public LoginWithEmailCommandHandler(
        IRepositoryBase<User, Guid> userRepository,
        IResponseCacheService responseCacheService,
        IJwtProviderService jwtProviderService,
        IPasswordHashService passwordHashService,
        IOptions<AuthSettings> authSettings)
    {
        _userRepository = userRepository;
        _responseCacheService = responseCacheService;
        _jwtProviderService = jwtProviderService;
        _passwordHashService = passwordHashService;
        _authSettings = authSettings.Value;
    }

    public async Task<Result<Success<LoginResponse>>> Handle(LoginWithEmailCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Tìm người dùng theo số điện thoại
        var user = await _userRepository.FindSingleAsync(
             u => u.Email == command.Email,
             includeProperties: q => q
                 .Include(u => u.AuthProviders)
                 .Include(u => u.UserRoles)
                     .ThenInclude(ur => ur.Role),
             cancellationToken: cancellationToken
         );

        // Bước 2: Trả về thất bại nếu không tìm thấy người dùng
        if (user == null)
            return FailureFromMessage(AuthErrors.AccountNotExistException);

        // Bước 3: Tìm số điện thoại và xác minh mật khẩu
        var provider = user.AuthProviders?
            .FirstOrDefault(p => p.ProviderType == AuthProviderType.Email && p.ProviderKey == command.Email);

        var isPasswordValid = provider != null && _passwordHashService.VerifyPassword(command.Password, provider.PasswordHash);
        if (!isPasswordValid)
            return FailureFromMessage(AuthErrors.PasswordMismatch);

        // Bước 4: Tạo JWT token và thông tin hồ sơ người dùng
        var loginResponse = GenerateLoginResponse(user);

        // Bước 5: Lưu refresh token vào cache
        await SaveRefreshTokenCacheAsync(user.Id, loginResponse!);

        // Bước 6: Trả về phản hồi thành công khi đăng nhập
        return Result.Success(new Success<LoginResponse>
            (AuthMessages.LoginSuccessful.GetMessage().Code, AuthMessages.LoginSuccessful.GetMessage().Message, loginResponse));
    }

    /// <summary>
    /// Tạo phản hồi đăng nhập bao gồm JWT token và thông tin người dùng.
    /// </summary>
    private LoginResponse GenerateLoginResponse(User user)
    {
        var roles = user.UserRoles
            .Select(ur => ur.Role.RoleType.ToString())
            .ToList() ?? [];

        var token = _jwtProviderService.GenerateToken(user.Id, roles);

        var userDto = new AuthUserDto(
            Id: user.Id.ToString(),
            FullName: user.FullName,
            AvatarUrl: user.Avatar?.Url,
            IsFirstUpdated: user.IsFirstUpdated,
            Roles: roles
        );

        return new LoginResponse(token, userDto);
    }

    /// <summary>
    /// Lưu refresh token vào cache để sử dụng
    /// </summary>
    private async Task SaveRefreshTokenCacheAsync(Guid userId, LoginResponse response)
    {
        var cacheKey = $"{AuthConstants.RefreshTokenCachePrefix}{userId}";
        var refreshToken = response.AuthToken.RefreshToken;

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _responseCacheService.SetCacheResponseAsync(cacheKey, refreshToken, TimeSpan.FromMinutes(_authSettings.RefreshTokenExpMinute));
        }
    }

    /// <summary>
    /// Tạo đối tượng Result thất bại từ enum AuthMessages.
    /// </summary>
    private static Result<Success<LoginResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<LoginResponse>>(error);
    }
}
