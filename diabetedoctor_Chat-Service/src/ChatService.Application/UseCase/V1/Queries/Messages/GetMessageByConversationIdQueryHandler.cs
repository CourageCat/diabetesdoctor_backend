using ChatService.Contract.Attributes;
using ChatService.Contract.DTOs.MessageDtos;
using ChatService.Contract.Services.Message.Queries;

namespace ChatService.Application.UseCase.V1.Queries.Messages;

public sealed class GetMessageByConversationIdQueryHandler(
    IMongoDbContext mongoDbContext,
    IOptions<AppDefaultSettings> settings)
    : IQueryHandler<GetMessageByConversationIdQuery, Response<CursorPagedResult<MessageResponseDto>>>
{
    public async Task<Result<Response<CursorPagedResult<MessageResponseDto>>>> Handle(GetMessageByConversationIdQuery request,
        CancellationToken cancellationToken)
    {
        var pageSize = request.Pagination.PageSize > 0 ? request.Pagination.PageSize : 50;

        var userId = UserId.Of(request.UserId);
        var isAllowed = await CheckConversationPermissionAsync(userId, request.ConversationId, cancellationToken);
        if (isAllowed.IsFailure)
        {
            return Result.Failure<Response<CursorPagedResult<MessageResponseDto>>>(isAllowed.Error);
        }

        var filters = BuildFilter(request);
        var sorter = Builders<Message>.Sort;
        var participantLookup = ParticipantLookup();
        var addParticipantInfoStage = AddParticipantInfo();
        var messageProjection = BuildMessageProjection(settings.Value.UserAvatarDefault);

        var messages = await mongoDbContext.Messages
            .Aggregate()
            .Match(filters)
            .Sort(sorter.Descending(m => m.Id))
            .Limit(pageSize + 1)
            .Lookup<Participant, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
                foreignCollection: mongoDbContext.Participants,
                let: new BsonDocument("senderId", "$sender_id"),
                lookupPipeline: participantLookup,
                @as: "participant_info")
            .AppendStage<BsonDocument>(addParticipantInfoStage)
            .Project<MessageResponseDto>(messageProjection)
            .ToListAsync(cancellationToken: cancellationToken);

        var hasNext = messages.Count > pageSize;

        if (hasNext)
        {
            messages.RemoveRange(pageSize, messages.Count - pageSize);
        }

        messages.Reverse();

        var result = CursorPagedResult<MessageResponseDto>.Create(messages, 0, pageSize,
            hasNext ? messages[^1].Id : string.Empty,
            hasNext);
        return Result.Success(new Response<CursorPagedResult<MessageResponseDto>>(
            MessageMessage.MessagesInConversation.GetMessage().Code,
            MessageMessage.MessagesInConversation.GetMessage().Message,
            result));
    }

    private async Task<Result> CheckConversationPermissionAsync(
        UserId userId,
        ObjectId conversationId,
        CancellationToken cancellationToken)
    {
        var isAllowed = await mongoDbContext.Participants.Find(
                p => p.UserId == userId 
                     && p.ConversationId == conversationId 
                     && p.ParticipantStatus == ParticipantStatus.Active 
                     && p.IsDeleted == false)
            .AnyAsync(cancellationToken);

        return isAllowed
            ? Result.Success()
            : Result.Failure(ConversationErrors.NotFound);
    }

    private static FilterDefinition<Message> BuildFilter(GetMessageByConversationIdQuery query)
    {
        var builder = Builders<Message>.Filter;
        var filters = new List<FilterDefinition<Message>>
        {
            builder.Eq(m => m.ConversationId, query.ConversationId)
        };

        if (!string.IsNullOrEmpty(query.Pagination.Cursor) && ObjectId.TryParse(query.Pagination.Cursor, out var cursor))
        {
            filters.Add(builder.Lt(m => m.Id, cursor));
        }

        return builder.And(filters);
    }
    
    private static PipelineDefinition<Participant, BsonDocument> ParticipantLookup()
    {
        return PipelineDefinition<Participant, BsonDocument>.Create(
            new BsonDocument("$match", new BsonDocument
            {
                { "$expr", new BsonDocument("$eq", new BsonArray { "$user_id", "$$senderId" }) }
            }),
            new BsonDocument("$limit", 1));
    }

    private static BsonDocument AddParticipantInfo()
    {
        return new BsonDocument
        {
            {
                "$addFields", new BsonDocument("participant_info",
                    new BsonDocument("$cond", new BsonDocument
                    {
                        {
                            "if",
                            new BsonDocument("$eq", new BsonArray { new BsonDocument("$size", "$participant_info"), 0 })
                        },
                        { "then", BsonNull.Value },
                        {
                            "else", 
                            new BsonDocument("$arrayElemAt", new BsonArray { "$participant_info", 0 })
                        }
                    })
                )
            }
        };
    }

    private static BsonDocument BuildMessageProjection(string avatarDefault)
    {
        return new BsonDocument
        {
            { "_id", 1 },
            { "content", 1 },
            { "type", 1 },
            { "file_attachment", 1 },
            { "created_date", 1 },
            {
                "participant_info", new BsonDocument
                {
                    { "_id", "$participant_info.user_id._id" },
                    {
                        "full_name", new BsonDocument("$ifNull", new BsonArray
                        {
                            "$participant_info.display_name",
                            "Người dùng không xác định"
                        })
                    },
                    {
                        "avatar", new BsonDocument("$ifNull", new BsonArray
                        {
                            "$participant_info.avatar.public_url",
                            avatarDefault
                        })
                    },
                    {
                        "role", new BsonDocument("$ifNull", new BsonArray
                        {
                            "$participant_info.role",
                            -1
                        })
                    }
                }
            }
        };
    }
}