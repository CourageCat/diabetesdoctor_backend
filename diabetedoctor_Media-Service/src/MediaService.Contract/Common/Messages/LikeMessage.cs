namespace MediaService.Contract.Common.Messages;

public enum LikeMessage
{
    [Message("Thích bài viết thành công.", "like_01")]
    LikePostSuccessfully,
    [Message("Hủy thích bài viết thành công.", "like_02")]
    UnLikePostSuccessfully,
}
