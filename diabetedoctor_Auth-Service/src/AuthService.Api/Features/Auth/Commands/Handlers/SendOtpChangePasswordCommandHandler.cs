using System.Security.Cryptography;
using AuthService.Api.Common.DomainErrors;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class SendOtpChangePasswordCommandHandler : ICommandHandler<SendOtpChangePasswordCommand, Success>
{
    private readonly IResponseCacheService _responseCacheService;
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly ITwilioService _twilioService;

    public SendOtpChangePasswordCommandHandler(IResponseCacheService responseCacheService, IRepositoryBase<User, Guid> userRepository, ITwilioService twilioService)
    {
        _responseCacheService = responseCacheService;
        _userRepository = userRepository;
        _twilioService = twilioService;
    }
    
    public async Task<Result<Success>> Handle(SendOtpChangePasswordCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra người dùng hợp lệ
        var userFound = await _userRepository.FindSingleAsync(user => user.Id == command.UserId, cancellationToken);
        if (userFound is null)
        {
            return FailureFromMessage(AuthErrors.AccountNotExistException);
        }
        
        // Bước 2: Sinh mã OTP gồm 6 chữ số
        var otp = GenerateOtp();
        
        // Bước 3: Lưu otp vào cache trong vòng 30 phút
        await _responseCacheService.SetCacheResponseAsync(
            cacheKey: $"{AuthConstants.ChangePasswordOtpCachePrefix}{userFound.PhoneNumber}",
            response: otp,
            timeOut: TimeSpan.FromMinutes(30));
        
        // Bước 4: Gửi Otp
        await _twilioService.SendOtp(otp);
        
        // Bước 5: Trả về kết quả thành công
        return Result.Success(new Success(
            AuthMessages.SendOtpChangePasswordSuccessfully.GetMessage().Code,
            AuthMessages.SendOtpChangePasswordSuccessfully.GetMessage().Message));
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