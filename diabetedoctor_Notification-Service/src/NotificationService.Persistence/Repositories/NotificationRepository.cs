using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Abstractions.Repositories;
using NotificationService.Domain.Models;

namespace NotificationService.Persistence.Repositories;

public class NotificationRepository(IMongoDbContext context) : RepositoryBase<Notification>(context), INotificationRepository;