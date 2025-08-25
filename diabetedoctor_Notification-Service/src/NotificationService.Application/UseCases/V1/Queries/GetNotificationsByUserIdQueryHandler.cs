using NotificationService.Contract.DTOs.NotificationDtos;
using NotificationService.Contract.DTOs.Responses;
using NotificationService.Contract.Services.Notification;
using NotificationService.Domain;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases.V1.Queries;


public class GetNotificationsByUserIdQueryHandler(IMongoDbContext mongoDbContext) : IQueryHandler<GetNotificationsByUserIdQuery, GetNotificationsResponse>
{
    public async Task<Result<GetNotificationsResponse>> Handle(GetNotificationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.Filter.PageSize is > 0 ? request.Filter.PageSize.Value : 10;

        var userExist = await mongoDbContext.Users
            .Find(user => user.UserId.Id == request.UserId)
            .AnyAsync(cancellationToken);

        if (!userExist)
        {
            throw new UserException.UserNotFoundException();
        }

        var builder = Builders<Notification>.Filter;
        var filters = new List<FilterDefinition<Notification>> { builder.Eq(noti => noti.UserId.Id, request.UserId) };  


        if(filters is null)
        {
            throw new UserException.UserNotFoundException();
        }

        if (!string.IsNullOrWhiteSpace(request.Filter.Cursor))
        {
            filters.Add(builder.Lt(notification => notification.Id, ObjectId.Parse(request.Filter.Cursor)));
        }

        var unreadFilters = builder.Eq(n => n.IsRead, false);


        //var total = (await _mongoDbContext.GetCollection<Notification>(nameof(Notification))
        //    .Aggregate()
        //    .Match(builder.And(filters))
        //    .AppendStage<BsonDocument>(new BsonDocument("$count", "total"))
        //    .FirstOrDefaultAsync(cancellationToken))?["total"].AsInt32 ?? 0;

        var unreadCount = await mongoDbContext.Notifications.CountDocumentsAsync(builder.And(filters.Append(unreadFilters)), cancellationToken: cancellationToken);

        var projection = new BsonDocument
        {
            { "_id", 1 },
            { "user_id", "$user_id._id" },
            { "type", "$notification_type" },
            { "is_read", 1 },
            { "read_at", new BsonDocument
                {
                    { "$dateAdd", new BsonDocument
                        {
                            { "startDate", "$read_at" },
                            { "unit", "hour" },
                            { "amount", 7 }
                        }
                    }
                }
            },
            { "received_at", new BsonDocument
                {
                    { "$dateAdd", new BsonDocument
                        {
                            { "startDate", "$received_at" },
                            { "unit", "hour" },
                            { "amount", 7 }
                        }
                    }
                }
            },
            { "payload", 1 }
        };
        var result = await mongoDbContext.Notifications
        .Aggregate()
        .Match(builder.And(filters))
        .Sort(Builders<Notification>.Sort.Descending(notification => notification.Id))
        .Limit(pageSize + 1)
        .Project(projection)
        .As<NotificationDto>()
        .ToListAsync(cancellationToken);

        var cursor = result.Count > 0 ? result[^1].Id : null;

        var hasNext = result.Count > pageSize;

        if (hasNext)
        {
            result.RemoveRange(pageSize, result.Count - pageSize);
        }

        return Result.Success(new GetNotificationsResponse { Notifications = PagedList<NotificationDto>.Create(result, unreadCount, pageSize, cursor!, hasNext) });
    }
}