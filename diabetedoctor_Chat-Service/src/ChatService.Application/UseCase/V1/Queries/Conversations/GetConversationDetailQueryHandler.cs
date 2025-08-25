using ChatService.Contract.Attributes;
using ChatService.Contract.DTOs.ConversationDtos.Responses;
using ChatService.Contract.Services.Conversation.Queries;

namespace ChatService.Application.UseCase.V1.Queries.Conversations;

public sealed class GetConversationDetailQueryHandler(
    IMongoDbContext mongoDbContext)
    : IQueryHandler<GetConversationDetailQuery, Response<ConversationResponseDto>>
{
    public async Task<Result<Response<ConversationResponseDto>>> Handle(GetConversationDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(request.UserId);

        var staff = await CheckStaffOrUserPermission(userId, request.Role, cancellationToken);
        if (staff.IsFailure)
        {
            return Result.Failure<Response<ConversationResponseDto>>(staff.Error);
        }
        
        var filter = BuildFilter(request, userId, staff.Value);
        var projection = BuildConversationProjection();
        
        var conversation = await mongoDbContext.Conversations
            .Aggregate()
            .Match(filter)
            .Project<ConversationResponseDto>(projection)
            .FirstOrDefaultAsync(cancellationToken);
        
        return conversation is not null
            ? Result.Success(new Response<ConversationResponseDto>(
            ConversationMessage.ConversationInDetail.GetMessage().Code,
            ConversationMessage.ConversationInDetail.GetMessage().Message,
            conversation))
            : Result.Failure<Response<ConversationResponseDto>>(ConversationErrors.DetailNotFound);
    }

    private async Task<Result<HospitalId?>> CheckStaffOrUserPermission(UserId userId, string role, CancellationToken cancellationToken)
    {
        if (role != nameof(Role.HospitalStaff)) return Result.Success<HospitalId?>(null);
        var projection = Builders<User>.Projection.Include(u => u.HospitalId).Exclude(u => u.Id);
        var user = await mongoDbContext.Users
            .Find(u => u.UserId == userId && u.IsDeleted == false && u.HospitalId != null)
            .Project<User>(projection)
            .FirstOrDefaultAsync(cancellationToken);
        
        return user is not null
            ? Result.Success(user.HospitalId)
            : Result.Failure<HospitalId?>(UserErrors.StaffNotFound);
    }
    
    private FilterDefinition<Conversation> BuildFilter(GetConversationDetailQuery query, UserId userId, HospitalId? hospitalId)
    {
        var builder = Builders<Conversation>.Filter;
        var filters = new List<FilterDefinition<Conversation>>
        {
            builder.Eq(c => c.Id, query.ConversationId),
            builder.Eq(c => c.ConversationType, ConversationType.Group),
            query.Role == nameof(Role.HospitalStaff)
                ? builder.Eq(c => c.HospitalId, hospitalId)
                : builder.ElemMatch(c => c.Members, m => m == userId)
        };

        return builder.And(filters);   
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