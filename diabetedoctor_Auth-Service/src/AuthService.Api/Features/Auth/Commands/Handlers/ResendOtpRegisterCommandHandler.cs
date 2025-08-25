using Newtonsoft.Json;
using System.Security.Cryptography;
using AuthService.Api.Common.DomainErrors;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

/// <summary>
/// Xử lý yêu cầu gửi lại mã OTP cho đăng ký người dùng.
/// </summary>
public sealed class ResendOtpRegisterCommandHandler
    : ICommandHandler<ResendOtpRegisterCommand, Success>
{
    private readonly IResponseCacheService _responseCacheService;

    public ResendOtpRegisterCommandHandler(IResponseCacheService responseCacheService)
    {
        _responseCacheService = responseCacheService;
    }

    public async Task<Result<Success>> Handle(ResendOtpRegisterCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Lấy thông tin đăng ký đã cache theo số điện thoại
        var cacheKey = $"{AuthConstants.RegisterOtpCachePrefix}{command.PhoneNumber}";
        var cachedData = await _responseCacheService.GetCacheResponseAsync(cacheKey);

        // Bước 2: Kiểm tra và giải mã dữ liệu cache
        if (string.IsNullOrWhiteSpace(cachedData))
            return FailureFromMessage(AuthErrors.OtpCodeExpired);

        if (!TryDeserialize(cachedData, out var userDto) || string.IsNullOrWhiteSpace(userDto?.PhoneNumber))
            return FailureFromMessage(AuthErrors.OtpCodeExpired);

        // Bước 3: Cập nhật mã OTP mới
        var updatedUserDto = userDto with { Otp = GenerateOtp() };

        // Bước 4: Lưu lại thông tin đã cập nhật vào cache, hết hạn sau 30 phút
        await _responseCacheService.SetCacheResponseAsync(
            cacheKey,
            updatedUserDto,
            TimeSpan.FromMinutes(30));

        // Bước 5: Trả về thành công
        return Result.Success(new Success(
            AuthMessages.VerificationCodeSent.GetMessage().Code,
            AuthMessages.VerificationCodeSent.GetMessage().Message));
    }

    /// <summary>
    /// Giải mã chuỗi JSON thành đối tượng UserDto.
    /// </summary>
    private static bool TryDeserialize(string json, out UserDto? dto)
    {
        try
        {
            dto = JsonConvert.DeserializeObject<UserDto>(json);
            return true;
        }
        catch
        {
            dto = null;
            return false;
        }
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
            int number = BitConverter.ToInt32(buffer) & 0x7FFFFFFF; // lấy số dương
            otp = number % 1_000_000;
        } while (otp < 100_000); // đảm bảo đủ 6 chữ số

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
