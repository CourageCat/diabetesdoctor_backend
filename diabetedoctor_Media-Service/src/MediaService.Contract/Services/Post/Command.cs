using MongoDB.Bson;

namespace MediaService.Contract.Services.Post;

public record CreateDraftPostCommand(string ModeratorId) : ICommand<Success<PostCreatedResponse>>;

public record UpdatePostCommand(
    ObjectId Id,
    string? Title,
    string? Content,
    string? ContentHtml,
    string? Thumbnail,
    List<string>? CategoryIds,
    List<string>? Images,
    string ModeratorId,
    string? DoctorId, bool IsDraft) : ICommand<Success>;

public record ReviewPostCommand(
    ObjectId Id, bool IsApproved, string? ReasonRejected) : ICommand<Success>;
    
public record DeletePostCommand(
    ObjectId Id, string ModeratorId) : ICommand<Success>;