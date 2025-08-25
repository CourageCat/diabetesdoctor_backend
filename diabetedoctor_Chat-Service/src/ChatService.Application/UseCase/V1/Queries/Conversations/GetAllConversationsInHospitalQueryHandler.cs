using ChatService.Contract.Attributes;
using ChatService.Contract.DTOs.ConversationDtos.Responses;
using ChatService.Contract.Enums;
using ChatService.Contract.Services.Conversation.Filters;
using ChatService.Contract.Services.Conversation.Queries;

namespace ChatService.Application.UseCase.V1.Queries.Conversations;

public sealed class GetAllConversationsInHospitalQueryHandler(
    IMongoDbContext mongoDbContext)
    : IQueryHandler<GetAllConversationsInHospitalQuery, Response<PagedResult<ConversationResponseDto>>>
{
    public async Task<Result<Response<PagedResult<ConversationResponseDto>>>> Handle(GetAllConversationsInHospitalQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.Pagination.PageIndex > 0 ? request.Pagination.PageIndex : 1;
        var pageSize = request.Pagination.PageSize > 0 ? request.Pagination.PageSize : 20;
        
        var user = await CheckHospitalOfStaff(request, cancellationToken);
        if (user.IsFailure)
        {
            return Result.Failure<Response<PagedResult<ConversationResponseDto>>>(user.Error);
        }
        
        var filters = BuildFilter(request.Filters, user.Value.HospitalId!);
        var total = await mongoDbContext.Conversations.CountDocumentsAsync(filters, cancellationToken: cancellationToken);
        if (total == 0)
        {
            var emptyResult = PagedResult<ConversationResponseDto>.CreateEmpty(pageIndex, pageSize);
            return Result.Success(new Response<PagedResult<ConversationResponseDto>>(
                ConversationMessage.HospitalConversations.GetMessage().Code,
                ConversationMessage.HospitalConversations.GetMessage().Message,
                emptyResult));
        }

        var sorter = BuildConversationSort(request.Filters);
        var projection = BuildConversationProjection();
        
        var conversations = await mongoDbContext.Conversations
            .Aggregate()
            .Match(filters)
            .Sort(sorter)
            .Skip((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .Project<ConversationResponseDto>(projection)
            .ToListAsync(cancellationToken);
        
        var result = PagedResult<ConversationResponseDto>.Create(conversations, pageIndex, pageSize, total);
        return Result.Success(new Response<PagedResult<ConversationResponseDto>>(
            ConversationMessage.HospitalConversations.GetMessage().Code,
            ConversationMessage.HospitalConversations.GetMessage().Message,
            result));
    }
    
    private async Task<Result<User>> CheckHospitalOfStaff(GetAllConversationsInHospitalQuery query, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(query.UserId);
        var projection = Builders<User>.Projection.Include(u => u.HospitalId).Exclude(u => u.Id);
        var user = await mongoDbContext.Users
            .Find(u => u.UserId == userId && u.IsDeleted == false)
            .Project<User>(projection)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (user is null) return Result.Failure<User>(UserErrors.NotFound);
        
        return user.HospitalId is null 
            ? Result.Failure<User>(HospitalErrors.HospitalNotFound)
            : Result.Success(user);
    }

    private static FilterDefinition<Conversation> BuildFilter(GetAllConversationsInHospitalFilters queryFilters, HospitalId hospitalId)
    {
        var builder = Builders<Conversation>.Filter;
        var filters = new List<FilterDefinition<Conversation>>
        {
            builder.Eq(c => c.HospitalId, hospitalId),
            builder.Eq(c => c.ConversationType, ConversationType.Group)
        };

        if (!string.IsNullOrWhiteSpace(queryFilters.Search))
        {
            var search = SearchHelper.NormalizeSearchText(queryFilters.Search);
            filters.Add(builder.Regex(c => c.Name, new BsonRegularExpression(search, "i")));
        }
        
        return builder.And(filters);
    }

    private static SortDefinition<Conversation> BuildConversationSort(GetAllConversationsInHospitalFilters filters)
    {
        var builder = Builders<Conversation>.Sort;
        var sorters = new List<SortDefinition<Conversation>>();
        
        var isAsc = filters.Direction == SortDirectionEnum.Asc; 
        switch (filters.SortBy.ToLower())
        {
            case "name":
                sorters.Add(isAsc
                    ? builder.Ascending(c => c.Name)
                    : builder.Descending(c => c.Name));
                break;
            default:
                sorters.Add(isAsc
                    ? builder.Ascending(c => c.CreatedDate)
                    : builder.Descending(c => c.CreatedDate));
                break;
        }
        sorters.Add(builder.Descending(c => c.Id));
        return builder.Combine(sorters);
    }

    private static BsonDocument BuildConversationProjection()
    {
        return new BsonDocument
        {
            { "_id", 1 },
            { "name", 1 },
            { "avatar", "$avatar.public_url" },
            { "type", 1 },
            { "member_count", new BsonDocument("$size", "$members") },
            { "modified_date", 1 }
        };
    }
}