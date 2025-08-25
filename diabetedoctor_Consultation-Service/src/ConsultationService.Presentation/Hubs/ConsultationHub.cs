using ConsultationService.Domain.Abstractions.Repositories;
using ConsultationService.Domain.ValueObjects;
using ConsultationService.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ConsultationService.Presentation.Hubs;

public class ConsultationHub(
    PresenceTracker tracker,
    ILogger<ConsultationHub> logger,
    IUserRepository userRepository)
    : Hub
{
    // public async Task<bool> IsUserOnline(string userId)
    // {
    //     return await tracker.IsOnline(userId);
    // }
    
    [Authorize]
    public async Task<bool> CallUser(string toUserId, object offer)
    {
        var connId = await tracker.GetConnectionId(toUserId);
        if (string.IsNullOrWhiteSpace(connId))
        {
            return false;
        }
        var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty;
        var user = await userRepository.FindSingleAsync(
            u => u.UserId == UserId.Of(userId) 
                 && u.IsDeleted == false);
        if (user == null) return false;
        await Clients.Client(connId).SendAsync("IncomingCall", userId, user.DisplayName, user.Avatar.ToString(), offer);
        return true;
    }
    
    [Authorize]
    public async Task<bool> AcceptCall(string toUserId, object answer)
    {
        var connId = await tracker.GetConnectionId(toUserId);
        if (string.IsNullOrWhiteSpace(connId))
        {
            return false;
        }
        var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty;
        await Clients.Client(connId).SendAsync("CallAccepted", userId, answer);
        return true;
    }
    
    [Authorize]
    public async Task<bool> DeclineCall(string toUserId)
    {
        var connId = await tracker.GetConnectionId(toUserId);
        if (string.IsNullOrWhiteSpace(connId))
        {
            return false;
        }
        var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty;
        await Clients.Client(connId).SendAsync("CallDeclined", userId);
        return true;
    }
    
    [Authorize]
    public async Task SendIceCandidate(string toUserId, object candidate)
    {
        var connId = await tracker.GetConnectionId(toUserId);
        if (!string.IsNullOrWhiteSpace(connId))
        {
            await Clients.Client(connId).SendAsync("ReceiveIceCandidate", candidate);
        }
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty;
        
        // logger.LogInformation("OnConnectedAsync");
        if (string.IsNullOrWhiteSpace(userId))
        {
            Context.Abort();
            // logger.LogInformation("Disconnected");
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
        if (exception is not null)
        {
            logger.LogError(exception, "Something went wrong in Socket");
        }
        await base.OnDisconnectedAsync(exception);
    }
    // public Task Register(string userId)
    // {
    //     Users[userId] = Context.ConnectionId;
    //     Console.WriteLine($"[REGISTER] {userId} -> {Context.ConnectionId}");
    //     return Task.CompletedTask;
    // }
    //
    // public async Task CallUser(string toUserId, object offer)
    // {
    //     if (Users.TryGetValue(toUserId, out var connId))
    //     {
    //         await Clients.Client(connId).SendAsync("IncomingCall", Context.ConnectionId, offer);
    //     }
    // }
    //
    // public async Task AnswerUser(string toConnectionId, object answer)
    // {
    //     await Clients.Client(toConnectionId).SendAsync("ReceiveAnswer", answer);
    // }
    //
    // public async Task SendIceCandidate(string toConnectionId, object candidate)
    // {
    //     await Clients.Client(toConnectionId).SendAsync("ReceiveIceCandidate", candidate);
    // }
    //
}