using ChatService.Contract.DTOs.ValueObjectDtos;
using ChatService.Contract.Enums;

namespace ChatService.Contract.Services.Conversation.IntegrationCommands;

public record UpdateLastMessageInConversationCommand : ICommand
{
    public ObjectId ConversationId { get; init; }
    public string? SenderId { get; init; }
    public ObjectId MessageId { get; init; }
    public string? MessageContent { get; init; }
    public MessageTypeEnum MessageType { get; init; }
    public FileAttachmentDto? FileAttachmentDto { get; init; }
    public DateTime CreatedDate { get; init; }
}