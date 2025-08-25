using NotificationService.Contract.DTOs.ValueObjectDtos;
using NotificationService.Contract.Enums;

namespace NotificationService.Contract.Services.Notification.Commands;

public record PushChatNotificationCommand : ICommand
{
    public string SenderId { get; init; } = null!;
    public string ConversationId { get; init; } = null!;
    public ConversationTypeEnum ConversationType {get; init; }
    public string MessageId { get; init; } = null!;
    public string? MessageContent { get; init; } = null!;
    public MessageTypeEnum MessageType { get; init; }
    public FileAttachmentDto? FileAttachment { get; init; }
    public DateTime CreatedDate { get; init; }
};