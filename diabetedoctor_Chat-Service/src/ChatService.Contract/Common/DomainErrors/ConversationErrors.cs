using ChatService.Contract.Attributes;
using ChatService.Contract.Common.Messages;

namespace ChatService.Contract.Common.DomainErrors;

public static class ConversationErrors
{
    // validation
    public static readonly Error SameAsCurrentName = Error.Validation(
        ConversationMessage.SameAsCurrentName.GetMessage().Code,
        ConversationMessage.SameAsCurrentName.GetMessage().Message);
    
    // error
    public static readonly Error NotFound = Error.NotFound(
        ConversationMessage.ConversationNotFound.GetMessage().Code,
        ConversationMessage.ConversationNotFound.GetMessage().Message);
    
    public static readonly Error DetailNotFound = Error.NotFound(
        ConversationMessage.ConversationDetailNotFound.GetMessage().Code,
        ConversationMessage.ConversationDetailNotFound.GetMessage().Message);
    
    public static readonly Error NotFoundOrAccessDenied = Error.NotFound(
        ConversationMessage.ConversationNotFoundOrAccessDenied.GetMessage().Code,
        ConversationMessage.ConversationNotFoundOrAccessDenied.GetMessage().Message);
    
    // public static readonly Error Forbidden = Error.Forbidden(
    //     ConversationMessage.ConversationAccessDenied.GetMessage().Code,
    //     ConversationMessage.ConversationAccessDenied.GetMessage().Message);
    
    public static readonly Error CannotRemoveMember = Error.Forbidden(
        ConversationMessage.CannotRemoveMember.GetMessage().Code,
        ConversationMessage.CannotRemoveMember.GetMessage().Message);
    
    public static readonly Error ThisConversationIsClosed = Error.Forbidden(
        ConversationMessage.ThisConversationIsClosed.GetMessage().Code,
        ConversationMessage.ThisConversationIsClosed.GetMessage().Message);
    
    public static readonly Error ThisConversationNotBelongToYourHospital = Error.Forbidden(
        ConversationMessage.ThisConversationNotBelongToYourHospital.GetMessage().Code,
        ConversationMessage.ThisConversationNotBelongToYourHospital.GetMessage().Message);
    
}