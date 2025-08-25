using System.Diagnostics;
using ChatService.Contract.Attributes;
using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.UserDtos;
using ChatService.Contract.Enums;
using ChatService.Contract.Services.User.Filters;
using ChatService.Contract.Services.User.Queries;

namespace ChatService.Application.UseCase.V1.Queries.Users;

public sealed class GetAvailableUsersForConversationQueryHandler(
    IMongoDbContext mongoDbContext)
    : IQueryHandler<GetAvailableUsersForConversationQuery, Response<PagedResult<UserResponseDto>>>
{
    public async Task<Result<Response<PagedResult<UserResponseDto>>>> Handle(GetAvailableUsersForConversationQuery request,
        CancellationToken cancellationToken)
    {
        var pageIndex = request.Pagination.PageIndex > 0 ? request.Pagination.PageIndex : 1;
        var pageSize = request.Pagination.PageSize > 0 ? request.Pagination.PageSize : 20;
        
        var filters = await BuildFilter(request.Filters, request.UserId, cancellationToken);
        
        var total = await mongoDbContext.Users.CountDocumentsAsync(filters, cancellationToken: cancellationToken);
   
        if (total == 0)
        {
            var emptyResult = PagedResult<UserResponseDto>.CreateEmpty(pageIndex, pageSize);
            return Result.Success(new Response<PagedResult<UserResponseDto>>(
                UserMessage.AvailableUsers.GetMessage().Code,
                UserMessage.AvailableUsers.GetMessage().Message,
                emptyResult));
        }
        
        var sorter = BuildUserSort();
        var participantLookup = ParticipantLookup(request.ConversationId);
        var addStatusStage = AddStatusStage();
        var userProjection = BuildUserProjection();
        
        var users = await mongoDbContext.Users
            .Aggregate()
            .Match(filters)
            .Sort(sorter)
            .Skip((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .Lookup<Participant, Participant, IEnumerable<Participant>, User>(
                foreignCollection: mongoDbContext.Participants,
                let: new BsonDocument("userId", "$user_id"),
                lookupPipeline: participantLookup,
                @as: "participant")
            .Unwind("participant", new AggregateUnwindOptions<User>
            {
                PreserveNullAndEmptyArrays = true
            })
            .AppendStage<BsonDocument>(addStatusStage)
            .Project(userProjection)
            .As<UserResponseDto>()
            .ToListAsync(cancellationToken);
        
        var result = PagedResult<UserResponseDto>.Create(users, pageIndex, pageSize, total);
        
        return Result.Success(new Response<PagedResult<UserResponseDto>>(
            UserMessage.AvailableUsers.GetMessage().Code,
            UserMessage.AvailableUsers.GetMessage().Message,
            result));
    }

    private async Task<FilterDefinition<User>> BuildFilter(GetAvailableUsersForConversationFilters queryFilters,
        string staffId, CancellationToken cancellationToken = default)
    {
        var role = queryFilters.Role.ToEnum<RoleEnum, Role>();
        var builder = Builders<User>.Filter;
        
        var filters = new List<FilterDefinition<User>> { builder.Eq(u => u.Role, role) };
        
        if (role is Role.HospitalStaff or Role.Doctor)
        {
            var userId = UserId.Of(staffId); 
            var projection = Builders<User>.Projection.Include(u => u.HospitalId).Exclude(u => u.Id);
            var user = await mongoDbContext.Users.Find(u => u.UserId == userId).Project<User>(projection).FirstOrDefaultAsync(cancellationToken);
            filters.Add(builder.Eq(u => u.HospitalId, user.HospitalId));   
        }
       

        if (!string.IsNullOrWhiteSpace(queryFilters.Search))
        {
            var search = SearchHelper.NormalizeSearchText(queryFilters.Search);
            filters.Add(builder.Or(
                builder.Regex(u => u.DisplayName, new BsonRegularExpression(search, "i")),
                builder.Regex(u => u.PhoneNumber, new BsonRegularExpression(search, "i")),
                builder.Regex(u => u.Email, new BsonRegularExpression(search, "i"))));
        }

        return builder.And(filters);
    }

    private static SortDefinition<User> BuildUserSort()
    {
        var builder = Builders<User>.Sort;
        return builder.Combine(
            builder.Ascending(u => u.FullName.FirstName),
            builder.Ascending(u => u.FullName.MiddleName),
            builder.Ascending(u => u.FullName.LastName),
            builder.Descending(u => u.Id)
        );
    }

    private static PipelineDefinition<Participant, Participant> ParticipantLookup(ObjectId conversationId)
    {
        return new EmptyPipelineDefinition<Participant>()
            .Match(new BsonDocument
            {
                {
                    "$expr", new BsonDocument
                    {
                        {
                            "$and", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray { "$user_id", "$$userId" }),
                                new BsonDocument("$eq", new BsonArray { "$conversation_id", conversationId })
                            }
                        }
                    }
                }
            })
            .Limit(1);
    }

    private static BsonDocument AddStatusStage()
    {
        return new BsonDocument("$addFields", new BsonDocument
        {
            {
                "status", new BsonDocument("$switch", new BsonDocument
                {
                    {
                        "branches", new BsonArray
                        {
                            // banned (user level)
                            new BsonDocument
                            {
                                { "case", new BsonDocument("$eq", new BsonArray { "$is_deleted", true }) },
                                { "then", ConversationUserStatusEnum.SystemBanned }
                            },

                            // available (no participant)
                            new BsonDocument
                            {
                                {
                                    "case", new BsonDocument("$not", new BsonArray
                                    {
                                        new BsonDocument("$ifNull", new BsonArray { "$participant._id", false })
                                    })
                                },
                                { "then", ConversationUserStatusEnum.Available }
                            },

                            // available (rejoinable)
                            new BsonDocument
                            {
                                {
                                    "case", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray { "$participant.is_deleted", true }),
                                        new BsonDocument("$eq",
                                            new BsonArray { "$participant.status", ParticipantStatus.Active })
                                    })
                                },
                                { "then", ConversationUserStatusEnum.Available }
                            },

                            // banned
                            new BsonDocument
                            {
                                {
                                    "case",
                                    new BsonDocument("$eq",
                                        new BsonArray { "$participant.status", ParticipantStatus.LocalBan })
                                },
                                { "then", ConversationUserStatusEnum.Banned }
                            }
                        }
                    },
                    { "default", ConversationUserStatusEnum.AlreadyInGroup }
                })
            }
        });
    }

    private static BsonDocument BuildUserProjection()
    {
        return new BsonDocument
        {
            { "_id", "$user_id._id" },
            { "avatar", "$avatar.public_url" },
            { "full_name", "$display_name" },
            { "phone_number" , 1},
            { "email", 1},
            { "status", 1 },
            { "role", 1 }
        };
    }
}