using ChatService.Contract.Attributes;
using ChatService.Contract.DTOs.ParticipantDtos;
using ChatService.Contract.Enums;
using ChatService.Contract.Services.Conversation.Filters;
using ChatService.Contract.Services.Participant.Queries;

namespace ChatService.Application.UseCase.V1.Queries.Participants;

public sealed class GetParticipantsByConversationQueryHandler(
    IMongoDbContext mongoDbContext)
    : IQueryHandler<GetParticipantsByConversationQuery, Response<PagedResult<ParticipantResponseDto>>>
{
    public async Task<Result<Response<PagedResult<ParticipantResponseDto>>>> Handle(
        GetParticipantsByConversationQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.Pagination.PageIndex > 0 ? request.Pagination.PageIndex : 1;
        var pageSize = request.Pagination.PageSize > 0 ? request.Pagination.PageSize : 20;

        var conversationPermission = await CheckConversationPermission(request, cancellationToken);
        if (conversationPermission.IsFailure)
        {
            return Result.Failure<Response<PagedResult<ParticipantResponseDto>>>(conversationPermission.Error);
        }

        var filters = BuildFilter(request);
        var total = await mongoDbContext.Participants.CountDocumentsAsync(filters,
            cancellationToken: cancellationToken);
        if (total == 0)
        {
            var emptyResult = PagedResult<ParticipantResponseDto>.CreateEmpty(pageIndex, pageSize);
            return Result.Success(new Response<PagedResult<ParticipantResponseDto>>(
                ParticipantMessage.ParticipantsInGroupConversation.GetMessage().Code,
                ParticipantMessage.ParticipantsInGroupConversation.GetMessage().Message,
                emptyResult));
        }

        var sorter = BuildParticipantSort(request.Filters);
        var userLookup = UserLookup();
        var participantProjection = BuildParticipantProjection();

        var participants = await mongoDbContext.Participants
            .Aggregate()
            .Match(filters)
            .Sort(sorter)
            .Skip((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .Lookup<User, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
                foreignCollection: mongoDbContext.Users,
                let: new BsonDocument("userId", "$user_id"),
                lookupPipeline: userLookup,
                @as: "participant_info")
            .Project<ParticipantResponseDto>(participantProjection)
            .ToListAsync(cancellationToken: cancellationToken);

        var result = PagedResult<ParticipantResponseDto>.Create(participants, pageIndex, pageSize, total);

        return Result.Success(new Response<PagedResult<ParticipantResponseDto>>(
            ParticipantMessage.ParticipantsInGroupConversation.GetMessage().Code,
            ParticipantMessage.ParticipantsInGroupConversation.GetMessage().Message,
            result));
    }


    private async Task<Result> CheckConversationPermission(GetParticipantsByConversationQuery query,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Of(query.UserId);
        var builder = Builders<Conversation>.Filter;
        var filters = new List<FilterDefinition<Conversation>>();
        if (query.Role == nameof(Role.HospitalStaff))
        {
            var projection = Builders<User>.Projection.Include(u => u.HospitalId).Exclude(u => u.Id);
            var staff = await mongoDbContext.Users
                .Find(u => u.UserId == userId && u.IsDeleted == false && u.HospitalId != null)
                .Project<User>(projection)
                .FirstOrDefaultAsync(cancellationToken);
            if (staff is null) return Result.Failure(UserErrors.StaffNotFound);

            filters.AddRange([
                builder.Eq(c => c.Id, query.ConversationId),
                builder.Eq(c => c.ConversationType, ConversationType.Group),
                builder.Eq(c => c.HospitalId, staff.HospitalId)
            ]);
        }
        else
        {
            filters.AddRange([
                builder.Eq(c => c.Id, query.ConversationId),
                builder.Eq(c => c.ConversationType, ConversationType.Group),
                builder.AnyEq(c => c.Members, userId)
            ]);
        }

        var isAllowed = await mongoDbContext.Conversations.Find(builder.And(filters))
            .AnyAsync(cancellationToken);
        return isAllowed ? Result.Success() : Result.Failure(ConversationErrors.NotFound);
    }

    private static FilterDefinition<Participant> BuildFilter(GetParticipantsByConversationQuery query)
    {
        var builder = Builders<Participant>.Filter;
        var filters = new List<FilterDefinition<Participant>>
        {
            builder.Eq(p => p.ConversationId, query.ConversationId),
            builder.Eq(p => p.IsDeleted, false)
        };

        if (query.Role is nameof(Role.Doctor) or nameof(Role.Patient))
        {
            filters.Add(builder.Eq(p => p.ParticipantStatus, ParticipantStatus.Active));
        }

        if (!string.IsNullOrWhiteSpace(query.Filters.Search))
        {
            var search = SearchHelper.NormalizeSearchText(query.Filters.Search);
            filters.Add(builder.Or(
                builder.Regex(p => p.DisplayName, new BsonRegularExpression(search, "i")),
                builder.Regex(p => p.PhoneNumber, new BsonRegularExpression(search, "i")),
                builder.Regex(p => p.Email, new BsonRegularExpression(search, "i"))));
        }

        return builder.And(filters);
    }

    private static SortDefinition<Participant> BuildParticipantSort(GetParticipantsByConversationIdFilters filters)
    {
        var builder = Builders<Participant>.Sort;
        var sorters = new List<SortDefinition<Participant>>
        {
            builder.Ascending(p => p.Role)
        };

        var isAsc = filters.Direction == SortDirectionEnum.Asc;
        switch (filters.SortBy.ToLower())
        {
            case "date":
                sorters.Add(isAsc
                    ? builder.Ascending(p => p.CreatedDate)
                    : builder.Descending(p => p.CreatedDate));
                break;

            default:
                sorters.Add(isAsc
                    ? builder.Ascending(p => p.FullName.FirstName)
                    : builder.Descending(p => p.FullName.FirstName));

                sorters.Add(builder.Ascending(p => p.FullName.MiddleName));
                sorters.Add(builder.Ascending(p => p.FullName.LastName));
                break;
        }

        sorters.Add(builder.Descending(p => p.Id));
        return builder.Combine(sorters);
    }

    private static PipelineDefinition<User, BsonDocument> UserLookup()
    {
        return PipelineDefinition<User, BsonDocument>.Create(
            new BsonDocument("$match", new BsonDocument
            {
                { "$expr", new BsonDocument("$eq", new BsonArray { "$user_id", "$$userId" }) }
            }),
            new BsonDocument("$limit", 1));
    }

    private static BsonDocument BuildParticipantProjection()
    {
        return new BsonDocument
        {
            { "_id", "$user_id._id" },
            { "conversation_id", 1 },
            { "full_name", "$display_name" },
            { "avatar", "$avatar.public_url" },
            { "phone_number", 1 },
            { "email", 1 },
            { "role", 1 },
            { "invited_by", "$invited_by._id" },
            { "status", 1 }
        };
    }
}