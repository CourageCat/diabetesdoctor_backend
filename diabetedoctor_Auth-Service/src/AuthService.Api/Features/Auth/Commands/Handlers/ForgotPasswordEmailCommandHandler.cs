using AuthService.Api.Common.DomainErrors;
using System.Security.Cryptography;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class ForgotPasswordEmailCommandHandler
    : ICommandHandler<ForgotPasswordEmailCommand, Success>
{
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IResponseCacheService _responseCacheService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordEmailCommandHandler
        (IRepositoryBase<User, Guid> userRepository,
        IResponseCacheService responseCacheService,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _responseCacheService = responseCacheService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<Success>> Handle(ForgotPasswordEmailCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra Email
        var userFound = await _userRepository.AnyAsync(u => u.Email == command.Email, cancellationToken);

        if (!userFound)
            return FailureFromMessage(AuthErrors.EmailNotFound);

        // Bước 2: Sinh mã OTP gồm 6 chữ số
        var otp = GenerateOtp();

        // Bước 3: Lưu otp vào cache trong vòng 30 phút
        await _responseCacheService.SetCacheResponseAsync(
            cacheKey: $"{AuthConstants.ResetPasswordOtpCachePrefix}{command.Email}",
            response: otp,
            timeOut: TimeSpan.FromMinutes(30));

        // Bước 4: Gửi otp
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendMailAsync(
                    command.Email,
                    "Quên mật khẩu",
                    "ForgotPasswordEmail.html",
                    new Dictionary<string, string>
                    {
                { "OTP", otp },
                { "Email", command.Email }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendMailAsync error: {ex.Message}");
            }
        });

        // Bước 5: Trả về kết quả thành công
        return Result.Success(new Success(
            AuthMessages.SendEmailForgotPasswordSuccessfully.GetMessage().Code,
            AuthMessages.SendEmailForgotPasswordSuccessfully.GetMessage().Message));
    }

    /// <summary>
    /// Sinh mã OTP gồm 6 chữ số (từ 100000 đến 999999).
    /// </summary>
    private static string GenerateOtp()
    {
        int otp;
        do
        {
            Span<byte> buffer = stackalloc byte[4];
            RandomNumberGenerator.Fill(buffer);
            int number = BitConverter.ToInt32(buffer) & 0x7FFFFFFF;
            otp = number % 1_000_000;
        } while (otp < 100_000); // Đảm bảo luôn đủ 6 chữ số

        return otp.ToString();
    }

    /// <summary>
    /// Tạo kết quả thất bại dựa trên enum AuthMessages.
    /// </summary>
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}
