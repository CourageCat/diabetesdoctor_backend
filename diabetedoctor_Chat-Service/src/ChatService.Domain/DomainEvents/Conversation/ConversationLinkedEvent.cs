namespace ChatService.Domain.DomainEvents.Conversation;

public record ConversationLinkedEvent(string ConversationId, string ConsultationId, bool IsOpened = false) : IDomainEvent;