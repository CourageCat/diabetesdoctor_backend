namespace AuthService.Api.Common.Messages;

public enum AuthMessages
{
    [Message("Đăng ký thành công.", "auth_01")]
    RegistrationSuccessful,
    [Message("Đăng nhập thành công.", "auth_02")]
    LoginSuccessful,
    [Message("Đổi mật khẩu thành công.", "auth_03")]
    PasswordChangedSuccessfully,
    [Message("Làm mới phiên đăng nhập thành công.", "auth_04")]
    RefreshTokenSuccessful,
    [Message("Đăng xuất thành công.", "auth_05")]
    LogoutSuccessful,
    [Message("Xác minh số điện thoại thành công.", "auth_06")]
    PhoneNumberVerifiedSuccessfully,
    [Message("Gửi otp đổi mật khẩu thành công. Vui lòng kiểm tra tin nhắn gửi về số điện thoại.", "auth_07")]
    SendOtpChangePasswordSuccessfully,
    [Message("Gửi otp quên mật khẩu thành công. Vui lòng kiểm tra tin nhắn gửi về số điện thoại.", "auth_08")]
    SendOtpForgotPasswordSuccessfully,
    [Message("Gửi otp thành công. Vui lòng kiểm tra tin nhắn gửi về email.", "auth_09")]
    SendEmailForgotPasswordSuccessfully,
    [Message("Đổi mật khẩu thành công.", "auth_10")]
    ChangePasswordSuccessfully,
    [Message("Lưu Fcm Token thành công", "auth_11")]
    SaveFcmTokenSuccessfully,
    
    
    [Message("Số điện thoại đã được sử dụng.", "auth_error_01")]
    PhoneNumberAlreadyExists,
    [Message("Số điện thoại không được phép để trống", "auth_error_02")]
    PhoneNumberNotFound,
    [Message("Số điện thoại chưa đăng kí!", "auth_error_03")]
    PhoneNumberNotRegistered,
    [Message("Mật khẩu không khớp.", "auth_error_04")]
    PasswordMismatch,
    [Message("Mã xác nhận đã được gửi. Vui lòng kiểm tra tin nhắn.", "auth_error_05")]
    VerificationCodeSent,
    [Message("Mã xác nhận đã hết hạn. Vui lòng thử lại.", "auth_error_06")]
    VerificationCodeExpired,
    [Message("Tài khoản đã bị khóa.", "auth_error_07")]
    UserBanned,
    [Message("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.", "auth_error_08")]
    SessionExpired,
    [Message("Sai định dạng số điện thoại.", "auth_error_09")]
    InvalidPhoneNumberOrPassword,
    [Message("Phiên đăng nhập đã hết hạn.", "auth_error_10")]
    LoginTimeout,
    [Message("Mã OTP không hợp lệ hoặc đã hết hạn", "auth_error_11")]
    InvalidOtpCode,
    [Message("Tài khoản đã tồn tại.", "auth_error_12")]
    AccountExistException,
    [Message("Tài khoản không tồn tại.", "auth_error_13")]
    AccountNotExistException,
    [Message("Mật khẩu phải có ít nhất 6 ký tự.", "auth_error_14")]
    PasswordTooShort,
    [Message("Mã OTP không tồn tại", "auth_error_15")]
    OtpCodeNotExist,
    [Message("Hết thời gian gửi lại OTP, vui lòng đăng ký lại", "auth_error_16")]
    OtpCodeExpired,
    [Message("Email không tồn tại", "auth_error_17")]
    EmailNotFound,
    [Message("Sai định dạng email.", "auth_error_18")]
    InvalidEmailOrPassword,
    [Message("Mật khẩu cũ không đúng!", "auth_error_19")]
    OldPasswordNotMatch,
}