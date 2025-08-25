using NotificationService.Domain.Models;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Abstractions.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<List<UserId>> GetUserIdsAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetDeviceIdsAsync(List<UserId>? userIds = default, CancellationToken cancellationToken = default);
}