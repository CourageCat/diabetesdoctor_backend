using System.Collections.Concurrent;

namespace ConsultationService.Presentation.Extensions;

public class PresenceTracker
{
    private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();
    
    public Task<bool> UserConnected(string userId, string connectionId)
    {
        var isFirstConnection = OnlineUsers.TryAdd(userId, connectionId);

        if (!isFirstConnection)
        {
            OnlineUsers[userId] = connectionId;
        }

        return Task.FromResult(isFirstConnection);
    }

    public Task<bool> UserDisconnected(string userId, string connectionId)
    {
        if (OnlineUsers.TryGetValue(userId, out var existingConnId) && existingConnId == connectionId)
        {
            OnlineUsers.TryRemove(userId, out _);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    
    public Task<bool> IsOnline(string userId)
    {
        return Task.FromResult(OnlineUsers.ContainsKey(userId));
    }

    public Task<List<string>> GetOnlineUsers()
    {
        return Task.FromResult(OnlineUsers.Keys.OrderBy(x => x).ToList());
    }
    
    public Task<string?> GetConnectionId(string userId)
    {
        OnlineUsers.TryGetValue(userId, out var connId);
        return Task.FromResult(connId);
    }

    // public Task<bool> UserConnected(string userId, string connectionId)
    // {
    //     var isOnline = OnlineUsers.ContainsKey(userId);
    //
    //     OnlineUsers.AddOrUpdate(userId,
    //         addValueFactory: _ => connectionId,
    //         updateValueFactory: (_, existingList) =>
    //         {
    //             lock (existingList)
    //             {
    //                 existingList.Add(connectionId);
    //             }
    //             return existingList;
    //         });
    //
    //     return Task.FromResult(!isOnline);
    // }

    // public Task<bool> UserDisconnected(string userId, string connectionId)
    // {
    //     if (!OnlineUsers.TryGetValue(userId, out var connections))
    //         return Task.FromResult(false);
    //
    //     lock (connections)
    //     {
    //         connections.Remove(connectionId);
    //         if (connections.Count == 0)
    //         {
    //             OnlineUsers.TryRemove(userId, out _);
    //             return Task.FromResult(true); // trở thành offline
    //         }
    //     }
    //
    //     return Task.FromResult(false); // vẫn còn connection khác
    // }

}
