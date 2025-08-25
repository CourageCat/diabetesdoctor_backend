using MediaService.Contract.Enumarations.Post;
using MediaService.Domain.Abstractions.Entities;
using MediaService.Domain.Enums;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Domain.Models;

public class Post : DomainEntity<ObjectId>
{
    [BsonElement("title")]
    public string Title { get; private set; } = default!;
    [BsonElement("title_normalize")]
    public string TitleNormalize { get; private set; } = default!;
    [BsonElement("content")]
    public string Content { get; private set; } = default!;
    [BsonElement("content_html")]
    public string ContentHtml { get; private set; } = null!;
    [BsonElement("view")]
    public int View { get; private set; }
    [BsonElement("like")]
    public int Like { get; private set; }
    [BsonElement("status")]
    public PostStatus Status { get; private set; }
    [BsonElement("reason_rejected")]
    public string? ReasonRejected { get; private set; }
    [BsonElement("tutorial_type")]
    public TutorialType? TutorialType { get; private set; }
    [BsonElement("thumbnail")]
    public Image? Thumbnail { get; private set; } = default!;
    [BsonElement("images")]
    public List<ObjectId> Images { get; private set; } = default!;
    [BsonElement("moderator_id")]
    public UserId ModeratorId { get; private set; } = default!;
    [BsonElement("doctor_id")]
    public UserId? DoctorId { get; private set; } = default!;

    public static Post Create(ObjectId id, string title, string titleNormalize, string content, string contentHtml, PostStatus status, Image thumbnail, List<ObjectId> images, UserId moderatorId, UserId doctorId)
    {
        return new Post
        {
            Id = id,
            Title = title,
            TitleNormalize = titleNormalize,
            Content = content,
            ContentHtml = contentHtml,
            View = 0,
            Like = 0,
            Status = status,
            Thumbnail = thumbnail,
            Images = images,
            ModeratorId = moderatorId,
            DoctorId = doctorId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false,
        };       
    }
    public static Post CreateForSeedData(ObjectId id, string title, string titleNormalize, string content, string contentHtml, Image thumbnail, List<ObjectId> images, UserId moderatorId, UserId doctorId, TutorialType? tutorialType)
    {
        return new Post
        {
            Id = id,
            Title = title,
            TitleNormalize = titleNormalize,
            Content = content,
            ContentHtml = contentHtml,
            View = 0,
            Like = 0,
            Status = PostStatus.Approved,
            TutorialType = tutorialType,
            Thumbnail = thumbnail,
            Images = images,
            ModeratorId = moderatorId,
            DoctorId = doctorId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false,
        };       
    }

    public static Post CreateEmpty(ObjectId id, UserId moderatorId)
    {
        return new Post
        {
            Id = id,
            Title = "",
            TitleNormalize = "",
            Content = "",
            ContentHtml = "",
            View = 0,
            Like = 0,
            Status = PostStatus.Drafted,
            Thumbnail = null,
            Images = new List<ObjectId>(),
            ModeratorId = moderatorId,
            DoctorId = null,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false,
        };
    }

    public void UpdateStatusPost(PostStatus status, string? reasonRejected)
    {
        Status = status;
        if (status == PostStatus.Rejected)
        {
            ReasonRejected = reasonRejected;
        }
    }
}
