using System.Text.Json;
using AuthService.Api.Common.DomainErrors;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Success>
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly IResponseCacheService _responseCacheService;
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IRepositoryBase<AuthProvider, Guid> _authRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(IPasswordHashService passwordHashService,
        IResponseCacheService responseCacheService, IRepositoryBase<User, Guid> userRepository,
        IRepositoryBase<AuthProvider, Guid> authRepository, IUnitOfWork unitOfWork)
    {
        _passwordHashService = passwordHashService;
        _responseCacheService = responseCacheService;
        _userRepository = userRepository;
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra người dùng hợp lệ
        var userFound = await _userRepository.FindSingleAsync(
            u => u.Id == command.UserId,
            includeProperties: q => q
                .Include(u => u.AuthProviders)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role),
            cancellationToken: cancellationToken
        );
        if (userFound is null)
        {
            return FailureFromMessage(AuthErrors.AccountNotExistException);
        }

        var provider = userFound.AuthProviders?
            .FirstOrDefault(p => p.ProviderType == AuthProviderType.Phone);
        if (provider is null)
        {
            return FailureFromMessage(AuthErrors.AccountNotExistException);
        }

        // Bước 2: Kiểm tra Otp
        var cachedData =
            await _responseCacheService.GetCacheResponseAsync(
                $"{AuthConstants.ChangePasswordOtpCachePrefix}{userFound.PhoneNumber}");
        if (string.IsNullOrEmpty(cachedData))
            return FailureFromMessage(AuthErrors.VerificationCodeExpired);
        var otp = JsonSerializer.Deserialize<string>(cachedData);
        if (otp != command.Otp)
            return FailureFromMessage(AuthErrors.InvalidOtpCode);

        // Bước 3: Kiểm tra mật khẩu cũ có đúng không
        if (!_passwordHashService.VerifyPassword(command.OldPassword, provider.PasswordHash))
        {
            return FailureFromMessage(AuthErrors.OldPasswordNotMatch);
        }

        // Bước 4: Đổi mật khẩu và lưu vào DB
        var newPasswordHash = _passwordHashService.HashPassword(command.NewPassword);
        provider.UpdatePassword(newPasswordHash);
        if (!userFound.IsFirstUpdated && userFound.UserRoles[0].Role.RoleType == RoleType.Doctor)
            userFound.MarkAsFirstUpdated();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Bước 5: Xóa cache
        await _responseCacheService.DeleteCacheResponseAsync(
            $"{AuthConstants.ChangePasswordOtpCachePrefix}{userFound.PhoneNumber}");

        // Bước 6: Trả về kết quả thành công
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