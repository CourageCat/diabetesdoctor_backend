using ChatService.Contract.Attributes;
using ChatService.Contract.Common.Messages;

namespace ChatService.Contract.Common.DomainErrors;

public static class ParticipantErrors
{
    public static readonly Error ParticipantNotExistOrBanned = Error.NotFound(
        ParticipantMessage.ParticipantNotExistOrBanned.GetMessage().Code,
        ParticipantMessage.ParticipantNotExistOrBanned.GetMessage().Message);

    public static readonly Error ParticipantNotFound = Error.NotFound(
        ParticipantMessage.ParticipantNotFound.GetMessage().Code,
        ParticipantMessage.ParticipantNotFound.GetMessage().Message);
    
    public static Error ParticipantAlreadyExistedOrBanned(object members) => Error.Conflict(
        ParticipantMessage.ParticipantsAlreadyExistedOrBanned.GetMessage().Code,
        members);
    
    public static readonly Error ParticipantIsBanned = Error.BadRequest(
        ParticipantMessage.ParticipantIsBanned.GetMessage().Code,
        ParticipantMessage.ParticipantIsBanned.GetMessage().Message);
    
    public static readonly Error ParticipantAlreadyExisted = Error.Conflict(
        ParticipantMessage.ParticipantAlreadyExisted.GetMessage().Code,
        ParticipantMessage.ParticipantAlreadyExisted.GetMessage().Message);
    
    public static readonly Error YouAreBanned = Error.Forbidden(
        ParticipantMessage.YouAreBanned.GetMessage().Code,
        ParticipantMessage.YouAreBanned.GetMessage().Message);
    
    public static readonly Error YouAlreadyInGroup = Error.Conflict(
        ParticipantMessage.YouAlreadyInGroup.GetMessage().Code,
        ParticipantMessage.YouAlreadyInGroup.GetMessage().Message);
}