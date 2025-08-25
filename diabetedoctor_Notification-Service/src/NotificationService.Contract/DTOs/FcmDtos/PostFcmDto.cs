namespace NotificationService.Contract.DTOs.FcmDtos;

public class PostFcmDto : BaseFcmDto
{
    public string PostId { get; init; }

    public override Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>
        {
            { "post_id", PostId },
            //{ "title", Title },
            { "body", Body },
            { "icon", Icon }
        };
    }
}