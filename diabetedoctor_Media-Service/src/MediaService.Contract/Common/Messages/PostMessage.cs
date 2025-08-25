namespace MediaService.Contract.Common.Messages;

public enum PostMessage
{
    [Message("Tạo nháp bài viết thành công.", "post_01")]
    CreateDraftPostSuccessfully,
    [Message("Cập nhật bài viết nháp thành công.", "post_03")]
    UpdatePostSuccessfully,
    [Message("Xóa bài viết thành công.", "post_04")]
    DeletePostSuccessfully,
    [Message("Toàn bộ bài viết:", "post_05")]
    GetAllPostsSuccessfully,
    [Message("Toàn bộ bài viết có lượt xem cao nhất:", "post_06")]
    GetTopViewPostsSuccessfully,
    [Message("Chi tiết bài viết:", "post_07")]
    GetPostByIdSuccessfully,
    [Message("Toàn bộ bài viết từ danh sách yêu thích: ", "post_08")]
    GetAllPostsFromBookMarkSuccessfully,
    [Message("Toàn bộ bài viết đã thích: ", "post_09")]
    GetAllLikePostsSuccessfully,
    [Message("Xuất bản bài viết thành công.", "post_10")]
    PublishedPostSuccessfully,
    [Message("Duyệt bài viết thành công.", "post_11")]
    ApprovedPostSuccessfully,
    [Message("Từ chối bài viết thành công.", "post_12")]
    RejectedPostSuccessfully,
    
    [Message("Không tìm thấy bài viết!", "post_error_01")]
    PostNotFoundException,
    [Message("Bài viết đã được xuất bản!", "post_error_02")]
    PostHasAlreadyBeenPublishedException,
    [Message("Bài viết đã được xem xét!", "post_error_03")]
    PostHasAlreadyBeenReviewedException,
    [Message("Bài viết chưa được xuất bản!", "post_error_04")]
    PostIsDraftedException,
    [Message("Bài viết này không phải của bạn!", "post_error_05")]
    PostIsNotBelongToStaffException,
    
}
