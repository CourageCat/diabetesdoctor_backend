using MediaService.Contract.Enumarations.Post;
using MongoDB.Bson;

namespace MediaService.Contract.Services.Post;
public static class Filter
{
    public record PostFilter(string? SearchContent, ObjectId[]? CategoryIds, Status? Status, TutorialEnum? TutorialType, string? UserId, string? ModeratorId, string? DoctorId);
}
