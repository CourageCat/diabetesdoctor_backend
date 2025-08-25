using ChatService.Domain.Enums;

namespace ChatService.Domain.Models;

public class Participant : DomainEntity<ObjectId>
{
    [BsonElement("user_id")]
    public UserId UserId { get; private set; } = null!;
    
    [BsonElement("conversation_id")]
    public ObjectId ConversationId { get; private set; }
    
    [BsonElement("full_name")]
    public FullName FullName { get; private set; } = null!;
    
    [BsonElement("display_name")]
    public string DisplayName { get; private set; } = null!;
    
    [BsonElement("avatar")]
    public Image Avatar { get; private set; } = null!;
    [BsonElement("phone_number")]
    public string? PhoneNumber { get; private set; }
    [BsonElement("email")]
    public string? Email { get; private set; }
    [BsonElement("role")]
    public MemberRole Role { get; private set; }
    
    [BsonElement("invited_by")]
    public UserId? InvitedBy { get; private set; } = null!;
    
    [BsonElement("status")]
    public ParticipantStatus ParticipantStatus { get; private set; }
    
    private Participant() { }

    private Participant(ObjectId id, UserId userId, ObjectId conversationId, FullName fullName, Image avatar, string? phoneNumber, string? email, MemberRole role, UserId? invitedBy)
    {
        Id = id;
        UserId = userId;
        ConversationId = conversationId;
        FullName = fullName;
        DisplayName = fullName.ToString();
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
        Role = role;
        InvitedBy = invitedBy;
        ParticipantStatus = ParticipantStatus.Active;
        CreatedDate = CurrentTimeService.GetCurrentTime();
        ModifiedDate = CurrentTimeService.GetCurrentTime();
        IsDeleted = false;
    }
    
    public static Participant CreateOwner(ObjectId id, UserId userId, ObjectId conversationId, FullName fullName, Image avatar, string? phoneNumber, string? email)
    {
        return new Participant(
            id: id,
            userId: userId,
            conversationId: conversationId,
            fullName: fullName,
            avatar: avatar,
            phoneNumber: phoneNumber,
            email: email,
            role: MemberRole.Owner,
            invitedBy: userId);
    }
    
    public static Participant CreateAdmin(ObjectId id, UserId userId, ObjectId conversationId, FullName fullName, Image avatar, string? phoneNumber, string? email, UserId invitedBy)
    {
        return new Participant(
            id: id,
            userId: userId,
            conversationId: conversationId,
            fullName: fullName,
            avatar: avatar,
            phoneNumber: phoneNumber,
            email: email,
            role: MemberRole.Admin,
            invitedBy: invitedBy);
    }
    
    public static Participant CreateDoctor(ObjectId id, UserId userId, ObjectId conversationId, FullName fullName, Image avatar, string? phoneNumber, string? email, UserId? invitedBy = null)
    {
        return new Participant(
            id: id,
            userId: userId,
            conversationId: conversationId,
            fullName: fullName,
            avatar: avatar,
            phoneNumber: phoneNumber,
            email: email,
            role: MemberRole.Doctor,
            invitedBy: invitedBy);
    }
    
    public static Participant CreateMember(ObjectId id, UserId userId, ObjectId conversationId, FullName fullName, Image avatar, string? phoneNumber, string? email, UserId? invitedBy = null)
    {
        return new Participant(
            id: id,
            userId: userId,
            conversationId: conversationId,
            fullName: fullName,
            avatar: avatar,
            phoneNumber: phoneNumber,
            email: email,
            role: MemberRole.Member,
            invitedBy: invitedBy);
    }
    
    public void Ban()
    {
        ParticipantStatus = ParticipantStatus.LocalBan;
        ModifiedDate = CurrentTimeService.GetCurrentTime();
    }

    public void Unban()
    {
        ParticipantStatus = ParticipantStatus.Active;
        ModifiedDate = CurrentTimeService.GetCurrentTime();
    }
}