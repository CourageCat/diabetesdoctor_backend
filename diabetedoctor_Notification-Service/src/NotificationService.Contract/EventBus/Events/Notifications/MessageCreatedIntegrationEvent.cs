using NotificationService.Contract.DTOs.ValueObjectDtos;
using NotificationService.Contract.Enums;

namespace NotificationService.Contract.EventBus.Events.Notifications;

public record MessageCreatedIntegrationEvent : IntegrationEvent
{
    // public SenderInfo Sender { get; init; } = null!;
    // public ConversationInfo Conversation { get; init; } = null!;
    // public string MessageId { get; init; } = null!;
    // public string MessageContent { get; init; } = null!;
    // public int MessageType { get; init; }
    // public FileAttachmentDto? FileAttachment { get; init; }
    //
    // public int Type { get; init; }
    // public DateTime CreatedDate { get; init; }
    
    public string SenderId { get; init; } = null!;
    public string ConversationId { get; init; } = null!;
    public ConversationTypeEnum ConversationType {get; init; }
    public string MessageId { get; init; } = null!;
    public string? MessageContent { get; init; } = null!;
    public MessageTypeEnum MessageType { get; init; }
    public FileAttachmentDto? FileAttachment { get; init; }
    public DateTime CreatedDate { get; init; }
}

public record SenderInfo
{
    public string? SenderId { get; init; }
    public string? FullName { get; init; }
    public string? Avatar { get; init; }
}

public record ConversationInfo
{
    public string? ConversationId { get; init; }
    // public string? ConversationName { get; init; }
    // public string? Avatar { get; init; }
    public int ConversationType {get; init; }
}