namespace ChatService.Contract.DTOs.ConversationDtos.Requests;

public record CreateGroupConversationRequest(string Name, string? AvatarId);