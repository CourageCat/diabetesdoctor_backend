using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.Contract.Helpers;
using NotificationService.Domain;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Abstractions.Repositories;
using NotificationService.Domain.Models;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Persistence.Repositories;

public class ConversationRepository(IMongoDbContext context) 
    : RepositoryBase<Conversation>(context), IConversationRepository
{
    public async Task<UpdateResult> UpdateConversationAsync(IClientSessionHandle session, ConversationId conversationId, string? name, Image? avatar, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Conversation>.Filter.Eq(conversation => conversation.ConversationId, conversationId);
        var options = new UpdateOptions { IsUpsert = false };
        var updates = new List<UpdateDefinition<Conversation>>
        {
            Builders<Conversation>.Update.Set(conversation => conversation.ModifiedDate, CurrentTimeService.GetCurrentTime())
        };
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            updates.Add(Builders<Conversation>.Update.Set(conversation => conversation.Name, name));
        }

        if (avatar != null)
        {
            updates.Add(Builders<Conversation>.Update.Set(conversation => conversation.Avatar, avatar));
        }
        
        return await DbSet.UpdateOneAsync(session, filter, Builders<Conversation>.Update.Combine(updates), options, cancellationToken);
    }

    public async Task<UpdateResult> AddMemberToConversationAsync(IClientSessionHandle session, ConversationId conversationId, List<UserId> memberIds, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Conversation>.Filter.Eq(conversation => conversation.ConversationId, conversationId);
        var options = new UpdateOptions { IsUpsert = false };
        var updates = new List<UpdateDefinition<Conversation>>
        {
            // Builders<Conversation>.Update.Set(conversation => conversation.ModifiedDate, CurrentTimeService.GetCurrentTime()),
            Builders<Conversation>.Update.AddToSetEach(conversation => conversation.Members, memberIds)
        };
        
        return await DbSet.UpdateOneAsync(session, filter, Builders<Conversation>.Update.Combine(updates), options, cancellationToken);
    }

    public async Task<UpdateResult> RemoveMemberFromConversationAsync(IClientSessionHandle session, ConversationId conversationId, UserId memberId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Conversation>.Filter.Eq(conversation => conversation.ConversationId, conversationId);
        var options = new UpdateOptions { IsUpsert = false };
        var updates = new List<UpdateDefinition<Conversation>>
        {
            Builders<Conversation>.Update.Set(conversation => conversation.ModifiedDate, CurrentTimeService.GetCurrentTime()),
            Builders<Conversation>.Update.Pull(conversation => conversation.Members, memberId)
        };
        
        return await DbSet.UpdateOneAsync(session, filter, Builders<Conversation>.Update.Combine(updates), options, cancellationToken);
    }
}