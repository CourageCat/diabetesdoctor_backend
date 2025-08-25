using ChatService.Domain.Abstractions.Repositories;
using ChatService.Domain.Enums;
using ChatService.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Presentation.Hubs;

public class ChatHub(
    PresenceTracker tracker,
    IParticipantRepository participantRepository)
    : Hub
{
    [Authorize]
    public async Task<bool> JoinRoomChat(string conversationId)
    {
        var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty;
        if (string.IsNullOrWhiteSpace(userId)) return false;
        
        if(!ObjectId.TryParse(conversationId, out var conversationObjectId)) return false;

        var isParticipantFound = await participantRepository.ExistsAsync(
            p => p.UserId == UserId.Of(userId)
                 && p.ConversationId == conversationObjectId
                 && p.ParticipantStatus == ParticipantStatus.Active
                 && p.IsDeleted == false);
        if(isParticipantFound is false) return false;

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        return true;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }
        var connectionId = Context.ConnectionId;
        await Clients.All.SendAsync("Connector: " +  $"{connectionId} has joined");
        await tracker.UserConnected(userId, connectionId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty;
        var connectionId = Context.ConnectionId;
        await tracker.UserDisconnected(userId, connectionId);
        await base.OnDisconnectedAsync(exception);
    }
    
}