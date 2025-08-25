using ChatService.Contract.Attributes;

namespace ChatService.Contract.Common.Messages;

public enum MessageMessage
{
    [Message("Lưu tin nhắn thành công", "message_1")]
    CreateMessageSuccessfully,
    [Message("Danh sách tin nhắn trong cuộc trò chuyện", "message_2")]
    MessagesInConversation,
    
    // error
    [Message("Không tìm thấy tệp đính kèm", "message_er_1")]
    CannotDetermineFile,
}