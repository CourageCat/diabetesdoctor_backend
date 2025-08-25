namespace MediaService.Contract.Common.Messages;

public enum BookMarkMessage
{
    [Message("Thêm bài viết vào danh sách yêu thích thành công.", "bookmark_01")]
    BookmarkedSuccessfully,
    [Message("Xóa bài viết khỏi danh sách yêu thích thành công.", "bookmark_02")]
    UnBookmarkedSuccessfully,
    [Message("Xóa toàn bộ bài viết khỏi danh sách yêu thích thành công.", "bookmark_03")]
    RemoveAllPostsFromBookMarkSuccessfully,
}
