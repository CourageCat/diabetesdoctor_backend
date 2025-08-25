namespace NotificationService.Contract.EventBus.Events.Conversations;

public record GroupMemberRemovedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public string MemberId { get; init; } = null!;
}