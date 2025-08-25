using ChatService.Contract.Attributes;
using ChatService.Contract.Common.Messages;

namespace ChatService.Contract.Exceptions.BusinessExceptions;

public static class ConversationExceptions
{
    public sealed class GroupNotFoundException() : NotFoundException(
        ConversationMessage.ConversationNotFound.GetMessage().Message, ConversationMessage.ConversationNotFound.GetMessage().Code);

    public sealed class GroupAccessDeniedException() : ForbiddenException(
        ConversationMessage.ConversationAccessDenied.GetMessage().Message, ConversationMessage.ConversationAccessDenied.GetMessage().Code);
    
    public sealed class CannotRemoveMemberException() : ForbiddenException(
        ConversationMessage.CannotRemoveMember.GetMessage().Message, ConversationMessage.CannotRemoveMember.GetMessage().Code);
    
    // Group Member
    
}