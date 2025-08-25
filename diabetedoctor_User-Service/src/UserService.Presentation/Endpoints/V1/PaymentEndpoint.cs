using Net.payOS.Types;
using UserService.Contract.Attributes;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Payments.Commands;
using UserService.Presentation.Extensions;

namespace UserService.Presentation.Endpoints.V1;

public static class PaymentEndpoint
{
    public const string ApiName = "payments";
    private const string BaseUrl = $"/user-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapPaymentApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("", HandleCreatePaymentBankingAsync)
            .WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        group.MapPost("/webhook", HandleWebhookAsync);

        return builder;
    }

    private static async Task<IResult> HandleCreatePaymentBankingAsync(ISender sender, IUserContext context,
        [FromBody] CreatePaymentBankingCommand request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleWebhookAsync(ISender sender, IUserContext context,
        [FromBody] WebhookType webhook)
    {
        var command = new WebhookCommand
        {
            WebhookType = webhook
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}