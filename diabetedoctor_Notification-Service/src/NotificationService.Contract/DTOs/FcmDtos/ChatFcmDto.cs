using System.Globalization;
using NotificationService.Contract.Enums;

namespace NotificationService.Contract.DTOs.FcmDtos;

public class ChatFcmDto : BaseFcmDto
{
    public string ConversationId { get; init; } = null!; // done
    public string? ConversationName { get; init; } = null!; // done
    public string ConversationAvatar { get; init; } = null!; // done
    public int ConversationType { get; init; }
    public string SenderId { get; init; } = null!;
    public string SenderName { get; init; } = null!;
    public string SenderAvatar { get; init; } = null!;
    public string MessageId { get; init; } = null!; // done
    public string? MessageContent { get; init; } = null!;
    public int MessageType { get; init; }
    public string? FileUrl { get; init; } = null!;
    public int? FileType { get; init; }
    public DateTime CreatedDate { get; init; }
    
    public override Dictionary<string, string?> GetData()
    {
        return new Dictionary<string, string?>
        {
            { "title", Title },
            { "body", Body },
            { "icon", Icon },
            { "conversationId", ConversationId },
            { "conversationName", ConversationName },
            { "conversationType", ConversationType.ToString() },
            { "avatar", ConversationAvatar },
            { "senderId", SenderId },
            { "senderName", SenderName },
            { "senderAvatar", SenderAvatar },
            { "messageId", MessageId},
            { "messageContent", MessageContent },
            { "messageType", MessageType.ToString() },
            { "fileUrl", FileUrl },
            { "fileType", FileType.ToString() },
            { "createdDate", CreatedDate.ToString(CultureInfo.InvariantCulture) }
        };
    }
}