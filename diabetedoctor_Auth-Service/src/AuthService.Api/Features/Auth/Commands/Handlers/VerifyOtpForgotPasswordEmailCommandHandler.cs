using AuthService.Api.Common.DomainErrors;
using Newtonsoft.Json;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class VerifyOtpForgotPasswordEmailCommandHandler
    : ICommandHandler<VerifyOtpForgotPasswordEmailCommand, Success>
{
    private readonly IRepositoryBase<AuthProvider, Guid> _authProviderRepository;
    private readonly IResponseCacheService _responseCacheService;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IUnitOfWork _unitOfWork;
    public VerifyOtpForgotPasswordEmailCommandHandler
        (IResponseCacheService responseCacheService,
        IPasswordHashService passwordHashService,
        IUnitOfWork unitOfWork,
        IRepositoryBase<AuthProvider, Guid> authProviderRepository)
    {
        _responseCacheService = responseCacheService;
        _passwordHashService = passwordHashService;
        _unitOfWork = unitOfWork;
        _authProviderRepository = authProviderRepository;
    }

    public async Task<Result<Success>> Handle(VerifyOtpForgotPasswordEmailCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Lấy dữ liệu đăng ký đã cache theo Email
        var cacheKey = AuthConstants.ResetPasswordOtpCachePrefix + command.Email;
        var cachedData = await _responseCacheService.GetCacheResponseAsync(cacheKey);

        // Bước 2: Kiểm tra dữ liệu cache có hợp lệ không (có dữ liệu và JSON deserialize thành công)
        if (string.IsNullOrWhiteSpace(cachedData))
            return FailureFromMessage(AuthErrors.InvalidOtpCode);

        var otpCache = JsonConvert.DeserializeObject<string>(cachedData);

        // Bước 3: So sánh OTP nhập vào với OTP trong cache
        if (string.IsNullOrEmpty(otpCache) || !otpCache.Equals(command.Otp))
            return FailureFromMessage(AuthErrors.InvalidOtpCode);

        // Bước 4: Kiểm tra email không tồn tại trong database chưa (tránh trùng lặp)
        var authProvider = await _authProviderRepository.FindSingleAsync(x => x.ProviderType == AuthProviderType.Email && x.ProviderKey == command.Email);
        if (authProvider == null)
            return FailureFromMessage(AuthErrors.AccountNotExistException);

        // Bước 5: Thay đổi mật khẩu
        var newPassword = _passwordHashService.HashPassword(command.Password);

        // Bước 6: Cập nhật
        authProvider.UpdatePassword(newPassword);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _responseCacheService.DeleteCacheResponseAsync(cacheKey);

        // Bước 7: Trả về kết quả thành công
        return Result.Success(new Success(
            AuthMessages.ChangePasswordSuccessfully.GetMessage().Code,
            AuthMessages.ChangePasswordSuccessfully.GetMessage().Message));
    }

    /// <summary>
    /// Tạo kết quả thất bại dựa trên enum AuthMessages.
    /// </summary>
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}
