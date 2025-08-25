using AuthService.Api.DependencyInjection.Extensions;
using AuthService.Api.Features.Auth.Commands;

namespace AuthService.Api.Endpoints.V1;

public static class AuthEndpoint
{
    public static IVersionedEndpointRouteBuilder MapAuthApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/auth-service/api/v{version:apiVersion}/auth").HasApiVersion(1);

        group.MapPost("/register-phone", HandleRegisterPhoneAsync);
        group.MapPost("/verify-otp-register", HandleVerifyOtpRegisterAsync);
        group.MapPost("/resend-otp-register", HandleResendOtpRegisterAsync);
        group.MapPost("/login-phone", HandleLoginWithPhoneNumberAsync);
        group.MapPost("/login-email", HandleLoginWithEmailAsync);
        group.MapPost("/fcm-token", HandleSaveFcmTokenAsync).WithMetadata(new RequireAuthenticatedAttribute());

        group.MapPost("/send-otp-change-password", HandleSendOtpChangePasswordAsync)
            .WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        group.MapPatch("/change-password", HandleChangePasswordAsync).WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        
        group.MapPost("/forgot-password", HandlePatientForgotPasswordAsync);
        group.MapPost("/forgot-password-email", HandleForgotPasswordAsync);
        group.MapPost("/verify-forgot-password", HandleVerifyForgotPasswordAsync);
        group.MapPatch("/reset-password", HandleResetPasswordAsync);
        
        group.MapPost("/refresh-token", HandleRefreshTokenAsync);
        group.MapDelete("/logout", HandleLogoutAsync)
                 .WithMetadata(new RequireAuthenticatedAttribute());
        
        return builder;
    }

    private static async Task<IResult> HandleRegisterPhoneAsync(
        ISender sender,
        [FromBody] RegisterWithPhoneCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleVerifyOtpRegisterAsync(
        ISender sender,
        [FromBody] VerifyOtpRegisterCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleResendOtpRegisterAsync(
        ISender sender,
        [FromBody] ResendOtpRegisterCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleRefreshTokenAsync(
        ISender sender,
        [FromBody] RefreshTokenCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleLoginWithPhoneNumberAsync(
        ISender sender,
        [FromBody] LoginWithPhoneNumberCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleLoginWithEmailAsync(
        ISender sender,
        [FromBody] LoginWithEmailCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }


    private static async Task<IResult> HandleLogoutAsync(
        ISender sender,
        IClaimsService claimsService)
    {
        var userIdHeader = claimsService.GetCurrentUserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(new LogoutCommand(userId));

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleSendOtpChangePasswordAsync(
        ISender sender,
        IClaimsService claimsService)
    {
        var userIdHeader = claimsService.GetCurrentUserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(new SendOtpChangePasswordCommand{UserId = userId});

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleChangePasswordAsync(
        ISender sender,
        IClaimsService claimsService,
        ChangePasswordCommand request)
    {
        var userIdHeader = claimsService.GetCurrentUserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };
        var result = await sender.Send(command);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandlePatientForgotPasswordAsync(
        ISender sender,
        ForgotPasswordCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleForgotPasswordAsync(
      ISender sender,
      ForgotPasswordEmailCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleVerifyForgotPasswordAsync(
    ISender sender,
    VerifyOtpForgotPasswordEmailCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleResetPasswordAsync(
        ISender sender,
        ResetPasswordCommand request)
    {
        var result = await sender.Send(request);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleSaveFcmTokenAsync(
        ISender sender,
        [FromBody] SaveFcmTokenCommand request,
        IClaimsService claimsService)
    {
        var userIdHeader = claimsService.GetCurrentUserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };
        var result = await sender.Send(command);

        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}