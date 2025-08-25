using Net.payOS.Types;

namespace UserService.Contract.Services.Payments.Commands;

public record WebhookCommand : ICommand<Success>
{
    public WebhookType WebhookType { get; init; } = null!;
}