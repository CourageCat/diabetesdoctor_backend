using System.Globalization;
using ChatService.Contract.Attributes;
using ChatService.Contract.DTOs.ConversationDtos.Responses;
using ChatService.Contract.Enums;
using ChatService.Contract.Services.Conversation.Queries;

namespace ChatService.Application.UseCase.V1.Queries.Conversations;

public sealed class GetConversationsByUserQueryHandler(
    IMongoDbContext mongoDbContext,
    IOptions<AppDefaultSettings> settings)
    : IQueryHandler<GetConversationsByUserQuery, Response<CursorPagedResult<ConversationResponseDto>>>
{
    public async Task<Result<Response<CursorPagedResult<ConversationResponseDto>>>> Handle(
        GetConversationsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var pageSize = request.Pagination.PageSize > 0 ? request.Pagination.PageSize : 20;
        var userId = UserId.Of(request.UserId);
        // var totalCount = await mongoDbContext.Conversations.CountDocumentsAsync(builder.And(filters), cancellationToken: cancellationToken);

        var filters = BuildFilter(request, userId);
        var sorter = BuildConversationSort();
        var messageParticipantLookup = MessageParticipantLookup();
        var addParticipantInfoStage = AddMessageParticipantInfo();
        var conversationProjection = request.Filters.Type == ConversationTypeEnum.Group 
            ? BuildGroupConversationProjection(settings.Value.UserAvatarDefault)
            : BuildPersonalConversationProjection(settings.Value.UserAvatarDefault);

        var aggregateFluent = mongoDbContext.Conversations
            .Aggregate()
            .Match(filters)
            .Sort(sorter)
            .Limit(pageSize + 1)
            .As<BsonDocument>();

        switch (request.Filters.Type)
        {
            case ConversationTypeEnum.Group:
                var groupParticipantLookup = GroupParticipantLookup();
                var canBeAccess = CanBeAccessed();
                var removeLastMessageIfNoAccess = RemoveLastMessageIfNoAccess();

                aggregateFluent = aggregateFluent
                    .Lookup<Participant, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
                        foreignCollection: mongoDbContext.Participants,
                        let: new BsonDocument("userId", userId.ToString()),
                        lookupPipeline: groupParticipantLookup,
                        @as: "participant")
                    .AppendStage<BsonDocument>(canBeAccess)
                    .AppendStage<BsonDocument>(removeLastMessageIfNoAccess)
                    .Lookup<Participant, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
                        foreignCollection: mongoDbContext.Participants,
                        let: new BsonDocument("senderId", "$last_message.sender_id"),
                        lookupPipeline: messageParticipantLookup,
                        @as: "participant_info")
                    .AppendStage<BsonDocument>(addParticipantInfoStage);
                break;

            default:
                var personalParticipantLookup = PersonalParticipantLookup();

                aggregateFluent = aggregateFluent
                    .Lookup<Participant, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
                        foreignCollection: mongoDbContext.Participants,
                        let: new BsonDocument
                        {
                            { "members", "$members" },
                            { "currentMemberId", userId.ToString() }
                        },
                        lookupPipeline: personalParticipantLookup,
                        @as: "other_member")
                    .Unwind("other_member", new AggregateUnwindOptions<BsonDocument>
                    {
                        PreserveNullAndEmptyArrays = true
                    })
                    .Lookup<Participant, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
                        foreignCollection: mongoDbContext.Participants,
                        let: new BsonDocument("senderId", "$last_message.sender_id"),
                        lookupPipeline: messageParticipantLookup,
                        @as: "participant_info")
                    .AppendStage<BsonDocument>(addParticipantInfoStage);
                break;
        }

        var conversations = await aggregateFluent
            .Project<ConversationResponseDto>(conversationProjection)
            .ToListAsync(cancellationToken);

        // .Lookup<Participant, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
        //     foreignCollection: mongoDbContext.Participants,
        //     let: new BsonDocument("userId", userId.ToString()),
        //     lookupPipeline: groupParticipantLookup,
        //     @as: "participant")
        // .AppendStage<BsonDocument>(canBeAccess)
        // .AppendStage<BsonDocument>(removeLastMessageIfNoAccess)
        // .Lookup<Participant, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
        //     foreignCollection: mongoDbContext.Participants,
        //     let: new BsonDocument("senderId", "$last_message.sender_id"),
        //     lookupPipeline: messageParticipantLookup,
        //     @as: "participant_info")
        // .AppendStage<BsonDocument>(addParticipantInfoStage)
        // .Project<ConversationResponseDto>(conversationProjection)
        // .ToListAsync(cancellationToken: cancellationToken);


        var hasNext = conversations.Count > pageSize;

        if (hasNext)
        {
            conversations.RemoveRange(pageSize, conversations.Count - pageSize);
        }

        var result = CursorPagedResult<ConversationResponseDto>.Create(conversations, 0, pageSize,
            hasNext ? conversations[^1].ModifiedDate.ToString("O", CultureInfo.InvariantCulture) : string.Empty,
            hasNext);

        return Result.Success(new Response<CursorPagedResult<ConversationResponseDto>>(
            ConversationMessage.UserGroupConversations.GetMessage().Code,
            ConversationMessage.UserGroupConversations.GetMessage().Message,
            result));
    }

    private static FilterDefinition<Conversation> BuildFilter(GetConversationsByUserQuery query, UserId userId)
    {
        var type = query.Filters.Type.ToEnum<ConversationTypeEnum, ConversationType>();
        var builder = Builders<Conversation>.Filter;
        var filters = new List<FilterDefinition<Conversation>>
        {
            builder.Eq(c => c.ConversationType, type),
            builder.ElemMatch(c => c.Members, id => id == userId)
        };

        if (!string.IsNullOrWhiteSpace(query.Pagination.Cursor)
            && DateTime.TryParseExact(
                query.Pagination.Cursor,
                "O",
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var cursor))
        {
            filters.Add(builder.Lt(c => c.ModifiedDate, cursor));
        }

        return builder.And(filters);
    }

    private static SortDefinition<Conversation> BuildConversationSort()
    {
        var builder = Builders<Conversation>.Sort;
        return builder.Combine(
            builder.Descending(c => c.ModifiedDate),
            builder.Descending(c => c.Id));
    }

    private static PipelineDefinition<Participant, BsonDocument> PersonalParticipantLookup()
    {
        return PipelineDefinition<Participant, BsonDocument>.Create(
            new BsonDocument("$match", new BsonDocument
            {
                {
                    "$expr", new BsonDocument("$and", new BsonArray
                    {
                        new BsonDocument("$in", new BsonArray
                        {
                            "$user_id._id", new BsonDocument("$map", new BsonDocument
                            {
                                { "input", "$$members" },
                                { "as", "m" },
                                { "in", "$$m._id" }
                            })
                        }),
                        new BsonDocument("$ne", new BsonArray { "$user_id._id", "$$currentMemberId" })
                    })
                }
            }),
            new BsonDocument("$limit", 1));
    }

    private static PipelineDefinition<Participant, BsonDocument> GroupParticipantLookup()
    {
        return PipelineDefinition<Participant, BsonDocument>.Create(
            new BsonDocument("$match", new BsonDocument
            {
                { "$expr", new BsonDocument("$eq", new BsonArray { "$user_id._id", "$$userId" }) }
            }),
            new BsonDocument("$limit", 1));
    }

    private static BsonDocument CanBeAccessed()
    {
        return new BsonDocument
        {
            {
                "$addFields", new BsonDocument("can_view",
                    new BsonDocument("$cond", new BsonDocument
                    {
                        {
                            "if",
                            new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", "$participant"), 0 })
                        },
                        { "then", true },
                        { "else", false }
                    })
                )
            }
        };
    }

    private static BsonDocument RemoveLastMessageIfNoAccess()
    {
        return new BsonDocument
        {
            {
                "$addFields", new BsonDocument("last_message",
                    new BsonDocument("$cond", new BsonDocument
                    {
                        {
                            "if", new BsonDocument("$eq", new BsonArray { "$can_view", false })
                        },
                        { "then", BsonNull.Value },
                        { "else", "$last_message" }
                    })
                )
            }
        };
    }

    private static PipelineDefinition<Participant, BsonDocument> MessageParticipantLookup()
    {
        return PipelineDefinition<Participant, BsonDocument>.Create(
            new BsonDocument("$match", new BsonDocument
            {
                { "$expr", new BsonDocument("$eq", new BsonArray { "$user_id", "$$senderId" }) }
            }),
            new BsonDocument("$limit", 1));
    }

    private static BsonDocument AddMessageParticipantInfo()
    {
        return new BsonDocument
        {
            {
                "$addFields", new BsonDocument("participant_info",
                    new BsonDocument("$cond", new BsonDocument
                    {
                        {
                            "if",
                            new BsonDocument("$eq", new BsonArray { "$last_message", BsonNull.Value })
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

    private static BsonDocument BuildPersonalConversationProjection(string avatarDefault)
    {
        return new BsonDocument
        {
            { "_id", 1 },
            {
                "other_user_id", new BsonDocument("$ifNull", new BsonArray
                {
                    "$other_member.user_id._id",
                    -1
                })
            },
            {
                "name", new BsonDocument("$ifNull", new BsonArray
                {
                    "$other_member.display_name",
                    "Người dùng không xác định"
                })
            },
            {
                "avatar", new BsonDocument("$ifNull", new BsonArray
                {
                    "$other_member.avatar.public_url",
                    avatarDefault
                })
            },
            { "type", 1 },
            { "status", 1 },
            {
                "last_message", new BsonDocument("$cond", new BsonDocument
                {
                    { "if", new BsonDocument("$eq", new BsonArray { "$last_message", BsonNull.Value }) },
                    { "then", BsonNull.Value },
                    {
                        "else", new BsonDocument
                        {
                            { "_id", "$last_message._id" },
                            { "content", "$last_message.content" },
                            { "type", "$last_message.message_type" },
                            { "file_attachment", "$last_message.file_attachment" },
                            { "created_date", "$last_message.created_date" },
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
                        }
                    }
                })
            },
            { "canview", true },
            { "modified_date", 1 }
        };
    }

    private static BsonDocument BuildGroupConversationProjection(string avatarDefault)
    {
        return new BsonDocument
        {
            { "_id", 1 },
            { "name", 1 },
            { "avatar", "$avatar.public_url" },
            { "type", 1 },
            { "status", 1 },
            {
                "last_message", new BsonDocument("$cond", new BsonDocument
                {
                    { "if", new BsonDocument("$eq", new BsonArray { "$last_message", BsonNull.Value }) },
                    { "then", BsonNull.Value },
                    {
                        "else", new BsonDocument
                        {
                            { "_id", "$last_message._id" },
                            { "content", "$last_message.content" },
                            { "type", "$last_message.message_type" },
                            { "file_attachment", "$last_message.file_attachment" },
                            { "created_date", "$last_message.created_date" },
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
                        }
                    }
                })
            },
            { "modified_date", 1 }
        };
    }
}