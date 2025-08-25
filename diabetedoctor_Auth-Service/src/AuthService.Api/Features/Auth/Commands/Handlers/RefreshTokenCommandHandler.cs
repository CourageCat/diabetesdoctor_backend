using AuthService.Api.Common.DomainErrors;
using AuthService.Api.Features.Auth.Responses;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

/// <summary>
/// Xử lý làm mới access token từ refresh token.
/// </summary>
public sealed class RefreshTokenCommandHandler
    : ICommandHandler<RefreshTokenCommand, Success<LoginResponse>>
{
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IJwtProviderService _jwtProviderService;
    private readonly IResponseCacheService _responseCacheService;
    private readonly AuthSettings _authSettings;

    public RefreshTokenCommandHandler(
        IRepositoryBase<User, Guid> userRepository,
        IJwtProviderService jwtProviderService,
        IResponseCacheService responseCacheService,
        IOptions<AuthSettings> authSettings)
    {
        _userRepository = userRepository;
        _jwtProviderService = jwtProviderService;
        _responseCacheService = responseCacheService;
        _authSettings = authSettings.Value;
    }

    public async Task<Result<Success<LoginResponse>>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Trích xuất UserId từ refresh token
        var userId = GetUserIdFromRefreshToken(command.RefreshToken);

        // Bước 2: Lấy thông tin người dùng và vai trò từ DB
        var user = await GetUserWithRolesAsync(userId, cancellationToken);
        if (user == null)
            return FailureFromMessage(AuthErrors.AccountNotExistException);

        // Bước 3: Tạo lại JWT mới và thông tin người dùng
        var response = GenerateLoginResponse(user);

        // Bước 5: Lưu refresh token vào cache
        await SaveRefreshTokenCacheAsync(user.Id, response);

        // Bước 5: Trả về phản hồi thành công
        return Result.Success(new Success<LoginResponse>(
            code: AuthMessages.RefreshTokenSuccessful.GetMessage().Code,
            message: AuthMessages.RefreshTokenSuccessful.GetMessage().Message,
            data: response
        ));
    }

    /// <summary>
    /// Xác thực refresh token và lấy UserId từ trong token.
    /// </summary>
    private Guid GetUserIdFromRefreshToken(string refreshToken)
    {
        var principal = _jwtProviderService.ValidateAndGetClaimRefreshToken(refreshToken);
        if (principal is null)
            throw new UnauthorizedAccessException("Refresh token không hợp lệ.");

        var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == AuthConstants.UserId);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong token.");

        return userId;
    }

    /// <summary>
    /// Lấy người dùng kèm danh sách vai trò từ cơ sở dữ liệu.
    /// </summary>
    private async Task<User> GetUserWithRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindSingleAsync(
            u => u.Id == userId,
            includeProperties: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role),
            cancellationToken: cancellationToken
        );

        return user;
    }

    /// <summary>
    /// Tạo phản hồi đăng nhập mới gồm JWT token và thông tin người dùng.
    /// </summary>
    private LoginResponse GenerateLoginResponse(User user)
    {
        var roles = user.UserRoles.Select(r => r.Role.RoleType.ToString()).ToList();

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
    /// Trả về kết quả thất bại dựa theo enum AuthMessages.
    /// </summary>
    private static Result<Success<LoginResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<LoginResponse>>(error);
    }
}
