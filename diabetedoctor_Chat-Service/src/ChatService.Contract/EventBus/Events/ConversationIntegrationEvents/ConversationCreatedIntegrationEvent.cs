using ChatService.Contract.Enums;

namespace ChatService.Contract.EventBus.Events.ConversationIntegrationEvents;

public record ConversationCreatedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public string ConversationName { get; init; } = null!;
    public ConversationTypeEnum ConversationType { get; init; }
    public string Avatar { get; init; } = null!;
    public IEnumerable<string> Members {get; init;} = null!;
    public bool IsOpened { get; init; }
    public string? ConsultationId { get; init; }
}