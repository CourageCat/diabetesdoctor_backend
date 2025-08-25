namespace NotificationService.Contract.EventBus.Events.Conversations;

public record GroupMembersAddedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public IEnumerable<string> Members { get; init; } = null!;
}