using MongoDB.Driver;
using NotificationService.Domain.Models;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Abstractions.Repositories;

public interface IConversationRepository : IRepositoryBase<Conversation>
{
    // Task<BsonDocument?> GetConversationWithParticipant(ObjectId conversationId, string userId, ConversationType type, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateConversationAsync(IClientSessionHandle session, ConversationId conversationId, string? name, Image? avatar, CancellationToken cancellationToken = default);
    Task<UpdateResult> AddMemberToConversationAsync(IClientSessionHandle session, ConversationId conversationId, List<UserId> memberIds, CancellationToken cancellationToken = default);
    Task<UpdateResult> RemoveMemberFromConversationAsync(IClientSessionHandle session, ConversationId conversationId, UserId memberId, CancellationToken cancellationToken = default);
}