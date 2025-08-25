using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.ConversationDtos.Requests;
using ChatService.Contract.Services.Conversation.Commands.GroupConversation;
using ChatService.Contract.Services.Conversation.Filters;
using ChatService.Contract.Services.Conversation.Queries;
using ChatService.Contract.Services.Participant.Commands;
using ChatService.Contract.Services.User.Filters;
using ChatService.Contract.Services.User.Queries;

namespace ChatService.Presentation.V1;

public static class HospitalConversationEndpoints
{
    public const string ApiName = "hospital-conversations";
    private const string BaseUrl = $"/api/v{{version:apiVersion}}/{ApiName}";
    
    public static IVersionedEndpointRouteBuilder MapHospitalConversationApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var hospitalConversations = builder.MapGroup(BaseUrl).HasApiVersion(1);
        
        hospitalConversations.MapGet("", GetHospitalConversations).RequireAuthorization().WithSummary("Get groups of a hospital");
        hospitalConversations.MapPost("", CreateConversation).RequireAuthorization().WithSummary("Create a new group");
        hospitalConversations.MapPatch("{conversationId}", UpdateConversation).RequireAuthorization().WithSummary("Update a group");
        hospitalConversations.MapDelete("{conversationId}", DeleteConversation).RequireAuthorization().WithSummary("Delete a group (executor is owner)");

        // participants
        hospitalConversations.MapPost("{conversationId}/members", AddMembersToGroup).RequireAuthorization().WithSummary("Add new members to the group");
        hospitalConversations.MapPost("{conversationId}/doctors", AddDoctorToGroup).RequireAuthorization().WithSummary("Add new doctor to the group");
        hospitalConversations.MapPost("{conversationId}/admins", AddAdminToGroup).RequireAuthorization().WithSummary("Add new admin to the group");
        hospitalConversations.MapPost("{conversationId}/join", JoinGroup).RequireAuthorization().WithSummary("Join a group by link (just patient)");
        hospitalConversations.MapDelete("{conversationId}/participants/{participantId}", RemoveGroupParticipant).RequireAuthorization();

        // users
        hospitalConversations.MapGet("{conversationId}/available-users", GetAvailableUsers).RequireAuthorization();

        return builder;
    }
    
    private static async Task<IResult> GetHospitalConversations(ISender sender, IClaimsService claimsService,
        [AsParameters] OffsetPaginationRequest offsetPaginationRequest,
        [AsParameters] GetAllConversationsInHospitalFilters filters)
    {
        var userId = claimsService.GetCurrentUserId;
        var query = new GetAllConversationsInHospitalQuery
        { 
            UserId = userId, 
            Pagination = offsetPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> CreateConversation(ISender sender, IClaimsService claimsService,
        [FromBody] CreateGroupConversationRequest request)
    {
        var ownerId = claimsService.GetCurrentUserId;
        var command = new CreateGroupConversationCommand
        {
            OwnerId = ownerId,
            Name = request.Name,
            AvatarId = request.AvatarId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> UpdateConversation(ISender sender, IClaimsService claimsService,
        ObjectId conversationId,
        [FromBody] UpdateGroupConversationRequest request)
    {
        var staffId = claimsService.GetCurrentUserId;
        var command = new UpdateGroupConversationCommand
        {
            StaffId = staffId,
            ConversationId = conversationId,
            Name = request.Name,
            AvatarId = request.AvatarId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> DeleteConversation(ISender sender, IClaimsService claimsService,
        ObjectId conversationId)
    {
        var staffId = claimsService.GetCurrentUserId;
        var command = new DeleteGroupConversationCommand
        {
            StaffId = staffId,
            ConversationId = conversationId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> AddMembersToGroup(ISender sender, IClaimsService claimsService,
        ObjectId conversationId,
        [FromBody] AddMembersToGroupRequest request)
    {
        var adminId = claimsService.GetCurrentUserId;
        var command = new AddMembersToGroupCommand
        {
            StaffId = adminId,
            ConversationId = conversationId,
            UserIds = request.UserIds
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> AddDoctorToGroup(ISender sender, IClaimsService claimsService,
        ObjectId conversationId,
        [FromBody] AddDoctorToGroupRequest request)
    {
        var adminId = claimsService.GetCurrentUserId;
        var command = new AddDoctorToGroupCommand
        {
            StaffId = adminId,
            ConversationId = conversationId,
            DoctorId = request.DoctorId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> AddAdminToGroup(ISender sender, IClaimsService claimsService,
        ObjectId conversationId,
        [FromBody] AddAdminToGroupRequest request)
    {
        var staffId = claimsService.GetCurrentUserId;
        var command = new AddAdminToGroupCommand
        {
            StaffId = staffId,
            ConversationId = conversationId,
            AdminId = request.AdminId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> JoinGroup(ISender sender, IClaimsService claimsService, ObjectId conversationId,
        [FromBody] JoinGroupRequest request)
    {
        var userId = claimsService.GetCurrentUserId;
        var command = new JoinGroupCommand
        {
            UserId = userId,
            ConversationId = conversationId,
            InvitedBy = userId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> RemoveGroupParticipant(ISender sender, IClaimsService claimsService,
        ObjectId conversationId, string participantId)
    {
        var staffId = claimsService.GetCurrentUserId;
        var command = new RemoveGroupParticipantCommand
        {
            StaffId = staffId,
            ConversationId = conversationId,
            MemberId = participantId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> GetAvailableUsers(ISender sender, IClaimsService claimsService,
        ObjectId conversationId,
        [AsParameters] OffsetPaginationRequest paginationRequest,
        [AsParameters] GetAvailableUsersForConversationFilters filters)
    {
        var userId = claimsService.GetCurrentUserId;
        var query = new GetAvailableUsersForConversationQuery
        {
            UserId = userId,
            ConversationId = conversationId,
            Pagination = paginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    } 

}