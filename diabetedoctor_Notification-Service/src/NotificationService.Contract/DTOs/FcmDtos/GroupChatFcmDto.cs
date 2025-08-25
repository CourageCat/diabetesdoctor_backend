namespace NotificationService.Contract.DTOs.FcmDtos;

public class GroupChatFcmDto : BaseFcmDto
{
    public string ConversationId { get; init; } = null!;
    public string ConversationName { get; init; } = null!;
    public string ConversationAvatar { get; init; } = null!;
    public string MessageId { get; init; } = null!;
    public string MessageContent { get; init; } = null!;
    public string SenderId { get; init; } = null!;
    public string SenderName { get; init; } = null!;

    public override Dictionary<string, string?> GetData()
    {
        return new Dictionary<string, string?>
        {
            { "title", Title },
            { "body", Body },
            { "icon", Icon },
            { "messageId", MessageId},
            { "conversationId", ConversationId },
            { "conversationName", ConversationName },
            { "avatar", ConversationAvatar},
            { "senderId", SenderId },
            { "senderName", SenderName },
        };
    }
}