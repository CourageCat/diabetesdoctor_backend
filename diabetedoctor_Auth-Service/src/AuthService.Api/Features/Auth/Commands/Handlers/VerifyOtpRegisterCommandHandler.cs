using AuthService.Api.Common.DomainErrors;
using AuthService.Api.Features.Auth.Responses;
using AuthService.Api.Infrastructures.EventBus.Events;
using Newtonsoft.Json;
using Role = AuthService.Api.Persistences.Data.Models.Role;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

/// <summary>
/// Xử lý xác thực OTP trong quá trình đăng ký người dùng.
/// Nếu OTP hợp lệ, tạo người dùng mới trong hệ thống và trả về phản hồi đăng nhập (JWT + thông tin user).
/// </summary>
public sealed class VerifyOtpRegisterCommandHandler
    : ICommandHandler<VerifyOtpRegisterCommand, Success<LoginResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResponseCacheService _responseCacheService;
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IRepositoryBase<Role, Guid> _roleRepository;
    private readonly IJwtProviderService _jwtProviderService;
    private readonly DefaultAvatarSettings _defaultAvatarSettings;
    private readonly AuthSettings _authSettings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IRepositoryBase<OutboxEvent, Guid> _outboxEventRepository;

    public VerifyOtpRegisterCommandHandler(IUnitOfWork unitOfWork, IResponseCacheService responseCacheService, IRepositoryBase<User, Guid> userRepository, IRepositoryBase<Role, Guid> roleRepository, IJwtProviderService jwtProviderService, IOptions<DefaultAvatarSettings> defaultAvatarConfigs, IOptions<AuthSettings> authConfigs, IOptions<KafkaSettings> kafkaConfigs, IRepositoryBase<OutboxEvent, Guid> outboxEventRepository)
    {
        _unitOfWork = unitOfWork;
        _responseCacheService = responseCacheService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtProviderService = jwtProviderService;
        _defaultAvatarSettings = defaultAvatarConfigs.Value;
        _authSettings = authConfigs.Value;
        _kafkaSettings = kafkaConfigs.Value;
        _outboxEventRepository = outboxEventRepository;
    }
    
    /// <summary>
    /// Xử lý xác thực OTP.
    /// 1. Lấy dữ liệu đăng ký đã cache theo số điện thoại.
    /// 2. Kiểm tra OTP hợp lệ.
    /// 3. Kiểm tra số điện thoại chưa được đăng ký.
    /// 4. Tạo user mới từ dữ liệu cache.
    /// 5. Lưu user vào database.
    /// 6. Xóa cache OTP đã dùng.
    /// 7. Tạo phản hồi đăng nhập (JWT token + user info).
    /// 8. Lưu refresh token vào cache để tái sử dụng.
    /// 9. Trả về kết quả thành công kèm phản hồi đăng nhập.
    /// </summary>
    public async Task<Result<Success<LoginResponse>>> Handle(VerifyOtpRegisterCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Lấy dữ liệu đăng ký đã cache theo số điện thoại
        var cacheKey = AuthConstants.RegisterOtpCachePrefix + command.PhoneNumber;
        var cachedData = await _responseCacheService.GetCacheResponseAsync(cacheKey);

        // Bước 2: Kiểm tra dữ liệu cache có hợp lệ không (có dữ liệu và JSON deserialize thành công)
        if (!TryDeserialize(cachedData, out var userDto) || string.IsNullOrWhiteSpace(userDto?.PhoneNumber))
            return FailureFromMessage(AuthErrors.InvalidOtpCode);

        // Bước 3: So sánh OTP nhập vào với OTP trong cache
        if (userDto.Otp != command.Otp)
            return FailureFromMessage(AuthErrors.InvalidOtpCode);

        // Bước 4: Kiểm tra số điện thoại đã tồn tại trong database chưa (tránh trùng lặp)
        var userExists = await _userRepository.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber, cancellationToken);
        if (userExists)
            return FailureFromMessage(AuthErrors.AccountExistException);

        // Bước 5: Tạo đối tượng user mới dựa trên dữ liệu cache
        var user = await CreateUserAsync(userDto, cancellationToken);

        // Bước 6: Thêm user mới vào DbContext và lưu thay đổi
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Bước 7: Xóa cache OTP đã sử dụng
        await _responseCacheService.DeleteCacheResponseAsync(cacheKey);

        // Bước 8: Tạo phản hồi đăng nhập
        var loginResponse = GenerateLoginResponse(user);

        // Bước 9: Lưu refresh token vào cache để tái sử dụng
        await SaveRefreshTokenCacheAsync(user.Id, loginResponse);

        // Bước 10: Trả về kết quả thành công cùng phản hồi đăng nhập
        return Result.Success(new Success<LoginResponse>(
            AuthMessages.RegistrationSuccessful.GetMessage().Code,
            AuthMessages.RegistrationSuccessful.GetMessage().Message,
            loginResponse
        ));
    }

    /// <summary>
    /// Tạo user mới dựa trên dữ liệu cache (UserDto).
    /// - Gán vai trò mặc định (bệnh nhân - Patient).
    /// - Gán avatar mặc định.
    /// - Thiết lập thông tin đăng nhập bằng số điện thoại và mật khẩu đã hash.
    /// </summary>
    private async Task<User> CreateUserAsync(UserDto userDto, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.FindSingleAsync(r => r.RoleType == RoleType.Patient, cancellationToken);
        if (role == null)
            throw new Exception($"Role '{RoleType.Patient}' không tồn tại trong hệ thống.");

        var avatar = new Image(_defaultAvatarSettings.AvatarId, _defaultAvatarSettings.AvatarUrl ?? string.Empty);

        var user = User.Create(email: "", avatar: avatar, phoneNumber: userDto.PhoneNumber!, id: userDto.Id);
        user.AddPhoneLogin(userDto.PhoneNumber!, userDto.PasswordHash!);
        user.AssignRole(role.Id);

        return user;
    }

    /// <summary>
    /// Tạo phản hồi đăng nhập chứa token JWT và thông tin người dùng.
    /// </summary>
    private LoginResponse GenerateLoginResponse(User user)
    {
        var roles = user.UserRoles.Select(x => x.Role.RoleType.ToString()).ToList();

        var token = _jwtProviderService.GenerateToken(user.Id, roles);

        var userInfo = new AuthUserDto(
            Id: user.Id.ToString(),
            FullName: user.FullName,
            AvatarUrl: user.Avatar?.Url ?? string.Empty,
            IsFirstUpdated: user.IsFirstUpdated,
            Roles: roles
        );

        return new LoginResponse(token, userInfo);
    }

    /// <summary>
    /// Lưu refresh token vào cache
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
    /// Giải mã chuỗi JSON thành đối tượng UserDto.
    /// </summary>
    private static bool TryDeserialize(string? json, out UserDto? dto)
    {
        dto = null;
        if (string.IsNullOrWhiteSpace(json)) return false;

        try
        {
            dto = JsonConvert.DeserializeObject<UserDto>(json);
            return dto != null;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Tạo Result thất bại dựa trên enum AuthMessages
    /// </summary>
    private static Result<Success<LoginResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<LoginResponse>>(error);
    }
}
