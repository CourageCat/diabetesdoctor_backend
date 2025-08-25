using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.ConversationDtos.Requests;
using ChatService.Contract.DTOs.MessageDtos;
using ChatService.Contract.Services.Conversation.Commands.GroupConversation;
using ChatService.Contract.Services.Conversation.Filters;
using ChatService.Contract.Services.Conversation.Queries;
using ChatService.Contract.Services.Message.Commands;
using ChatService.Contract.Services.Message.Queries;
using ChatService.Contract.Services.Participant.Commands;
using ChatService.Contract.Services.Participant.Queries;
using ChatService.Contract.Services.User.Filters;
using ChatService.Contract.Services.User.Queries;

namespace ChatService.Presentation.V1;

public static class ConversationEndpoints
{
    public const string ApiName = "conversations";
    private const string BaseUrl = $"/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapConversationApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var conversations = builder.MapGroup(BaseUrl).HasApiVersion(1);

        // conversations
        conversations.MapGet("", GetUserConversations).RequireAuthorization().WithSummary("Get groups of a user");
        conversations.MapGet("{conversationId}", GetConversationDetail).RequireAuthorization().WithSummary("Get conversation detail");
        
        // participants
        conversations.MapGet("{conversationId}/participants", GetParticipantsInConversation).RequireAuthorization();
        conversations.MapDelete("{conversationId}/participants/me", LeaveGroup).RequireAuthorization().WithSummary("Leave conversation");

        // messages
        conversations.MapPost("{conversationId}/messages", CreateMessage).RequireAuthorization().WithSummary("Creates a new message");
        conversations.MapGet("{conversationId}/messages", GetMessagesInConversation).RequireAuthorization().WithSummary("Gets all messages");
        
        return builder;
    }

    private static async Task<IResult> GetConversationDetail(ISender sender, IClaimsService claimsService,
        ObjectId conversationId)
    {
        var userId = claimsService.GetCurrentUserId;
        var role = claimsService.GetCurrentRole;
        var query = new GetConversationDetailQuery
        {
            UserId = userId,
            Role = role,
            ConversationId = conversationId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> GetUserConversations(ISender sender, IClaimsService claimsService,
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetUserConversationsByUserFilters filters)
    {
        var userId = claimsService.GetCurrentUserId;
        var query = new GetConversationsByUserQuery 
        { 
            UserId = userId, 
            Pagination = cursorPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> GetParticipantsInConversation(ISender sender, IClaimsService claimsService,
        ObjectId conversationId,
        [AsParameters] OffsetPaginationRequest offsetPaginationRequest,
        [AsParameters] GetParticipantsByConversationIdFilters filters)
    {
        var userId = claimsService.GetCurrentUserId;
        var role = claimsService.GetCurrentRole;
        var query = new GetParticipantsByConversationQuery
        {
            UserId = userId,
            ConversationId = conversationId,
            Role = role,
            Pagination = offsetPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> LeaveGroup(ISender sender, IClaimsService claimsService,
        ObjectId conversationId)
    {
        var userId = claimsService.GetCurrentUserId;
        var command = new LeaveGroupCommand()
        {
            ConversationId = conversationId,
            UserId = userId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> CreateMessage(ISender sender, IClaimsService claimsService, 
        ObjectId conversationId,
        [FromBody] MessageCreateDto dto)
    {
        var userId = claimsService.GetCurrentUserId;
        var result = await sender.Send(new CreateMessageCommand
        {
            ConversationId = conversationId, ConversationType = dto.ConversationType, UserId = userId, 
            Content = dto.Content,
            MediaId = dto.MediaId,
            MessageType = dto.MessageType
        });
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> GetMessagesInConversation(ISender sender, IClaimsService claimsService,
        ObjectId conversationId,
        [AsParameters] CursorPaginationRequest cursorPaginationRequest)
    {
        var userId = claimsService.GetCurrentUserId;
        var result = await sender.Send(new GetMessageByConversationIdQuery
        {
            UserId = userId,
            ConversationId = conversationId, 
            Pagination = cursorPaginationRequest
        });
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();    
    }
}