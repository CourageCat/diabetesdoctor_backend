using ChatService.Domain.Enums;

namespace ChatService.Domain.DomainEvents.Conversation;

public record ConversationCreatedEvent(string ConversationId, string ConversationName, ConversationType ConversationType, string Avatar, IEnumerable<string> MemberIds, bool IsOpened = false, string? ConsultationId = null) : IDomainEvent;