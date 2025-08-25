using ChatService.Contract.Attributes;

namespace ChatService.Contract.Common.Messages;

public enum ParticipantMessage
{
    // handler
    [Message("Danh sách thành viên nhóm", "participant_1")]
    ParticipantsInGroupConversation,
    
    // error
    [Message("Thành viên không thuộc nhóm hoặc đã bị cấm", "participant_er_1")]
    ParticipantNotExistOrBanned,
    [Message("Thành viên không tồn tại", "participant_er_2")]
    ParticipantNotFound,
    [Message("Thành viên đã tồn tại hoặc đã bị cấm", "participant_er_3")]
    ParticipantsAlreadyExistedOrBanned,
    [Message("Thành viên này đang bị cấm khỏi hệ thống", "participant_er_8")]
    ParticipantIsBanned,
    [Message("Thành viên này đã tồn tại", "participant_er_8")]
    ParticipantAlreadyExisted,
    [Message("Bạn đã bị cấm khỏi nhóm này", "participant_er_10")]
    YouAreBanned,
    [Message("Bạn đã là thành viên trong nhóm", "participant_er_11")]
    YouAlreadyInGroup,
    
}