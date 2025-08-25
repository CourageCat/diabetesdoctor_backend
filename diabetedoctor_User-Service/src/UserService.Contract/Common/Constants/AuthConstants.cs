namespace UserService.Contract.Common.Constants;

public sealed class AuthConstants
{
    public const string BearerTokenScheme = "Bearer";
    public const string AccessToken = "AccessToken";
    public const string RefreshToken = "RefreshToken";
    public const string UserId = "UserId";
    public const string RefreshTokenCachePrefix = "authrefreshtoken:";
    public const string RegisterOtpCachePrefix = "authregister_";
    public const string PhoneNumberCachePrefix = "phonenumber_";
}