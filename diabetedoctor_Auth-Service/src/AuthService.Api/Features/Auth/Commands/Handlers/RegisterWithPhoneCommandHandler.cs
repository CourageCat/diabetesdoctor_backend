using System.Security.Cryptography;
using AuthService.Api.Common.DomainErrors;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

/// <summary>
/// Xử lý đăng ký người dùng bằng số điện thoại.
/// - Kiểm tra số điện thoại đã tồn tại chưa.
/// - Sinh mã OTP.
/// - Lưu thông tin đăng ký tạm thời vào cache chờ xác minh OTP.
/// </summary>
public sealed class RegisterWithPhoneCommandHandler
    : ICommandHandler<RegisterWithPhoneCommand, Success>
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IResponseCacheService _responseCacheService;
    private readonly ITwilioService _twilioService;

    public RegisterWithPhoneCommandHandler(IPasswordHashService passwordHashService, IRepositoryBase<User, Guid> userRepository, IResponseCacheService responseCacheService, ITwilioService twilioService)
    {
        _passwordHashService = passwordHashService;
        _userRepository = userRepository;
        _responseCacheService = responseCacheService;
        _twilioService = twilioService;
    }

    public async Task<Result<Success>> Handle(RegisterWithPhoneCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra số điện thoại đã tồn tại chưa
        var existing = await _userRepository.FindSingleAsync(
            u => u.PhoneNumber == command.PhoneNumber,
            includeProperties: u => u.AuthProviders, cancellationToken: cancellationToken);

        if (existing != null)
            return FailureFromMessage(AuthErrors.PhoneNumberAlreadyExists);

        // Bước 2: Sinh mã OTP gồm 6 chữ số
        var otp = GenerateOtp();

        // Bước 3: Map dữ liệu command và OTP thành UserDto
        var userDto = MapToUserDto(command, otp);

        // Bước 4: Lưu otp vào cache trong vòng 30 phút
        await _responseCacheService.SetCacheResponseAsync(
            cacheKey: $"{AuthConstants.RegisterOtpCachePrefix}{command.PhoneNumber}",
            response: userDto,
            timeOut: TimeSpan.FromMinutes(30));
        
        // Bước 5: Lưu sđt vào cache trong vòng 365 ngày
        await _responseCacheService.SetCacheResponseAsync(
            cacheKey: $"{AuthConstants.PhoneNumberCachePrefix}{userDto.Id}",
            response: command.PhoneNumber,
            timeOut: TimeSpan.FromDays(365));
        
        // Bước 6: Gửi Otp
        await _twilioService.SendOtp(otp);
        
        // Bước 6: Trả về kết quả thành công
        return Result.Success(new Success(
            AuthMessages.VerificationCodeSent.GetMessage().Code,
            AuthMessages.VerificationCodeSent.GetMessage().Message));
    }

    /// <summary>
    /// Chuyển dữ liệu đăng ký và OTP thành đối tượng UserDto.
    /// </summary>
    private UserDto MapToUserDto(RegisterWithPhoneCommand command, string otp)
    {
        var passwordHash = _passwordHashService.HashPassword(command.Password);
        return new UserDto(
            Id: Guid.NewGuid(),
            PhoneNumber: command.PhoneNumber,
            PasswordHash: passwordHash,
            Otp: otp
        );
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
    /// Trả về kết quả thất bại dựa theo enum AuthMessages.
    /// </summary>
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}
