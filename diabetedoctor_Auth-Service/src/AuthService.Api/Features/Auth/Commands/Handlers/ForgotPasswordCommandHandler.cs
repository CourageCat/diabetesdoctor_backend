
using System.Security.Cryptography;
using AuthService.Api.Common.DomainErrors;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class ForgotPasswordCommandHandler
    : ICommandHandler<ForgotPasswordCommand, Success>
{
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IResponseCacheService _responseCacheService;
    private readonly ITwilioService _twilioService;

    public ForgotPasswordCommandHandler
        (IRepositoryBase<User, Guid> userRepository,
        IResponseCacheService responseCacheService, ITwilioService twilioService)
    {
        _userRepository = userRepository;
        _responseCacheService = responseCacheService;
        _twilioService = twilioService;
    }

    public async Task<Result<Success>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra số điện thoại
        var userFound = await _userRepository.AnyAsync(u => u.PhoneNumber == command.PhoneNumber, cancellationToken);

        if (!userFound)
            return FailureFromMessage(AuthErrors.PhoneNumberNotRegistered);
        
        // Bước 2: Sinh mã OTP gồm 6 chữ số
        var otp = GenerateOtp();
        
        // Bước 3: Lưu otp vào cache trong vòng 30 phút
        await _responseCacheService.SetCacheResponseAsync(
            cacheKey: $"{AuthConstants.ResetPasswordOtpCachePrefix}{command.PhoneNumber}",
            response: otp,
            timeOut: TimeSpan.FromMinutes(30));
        
        // Bước 4: Gửi otp
        await _twilioService.SendOtp(otp);
        
        // Bước 5: Trả về kết quả thành công
        return Result.Success(new Success(
            AuthMessages.SendOtpForgotPasswordSuccessfully.GetMessage().Code,
            AuthMessages.SendOtpForgotPasswordSuccessfully.GetMessage().Message));
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
