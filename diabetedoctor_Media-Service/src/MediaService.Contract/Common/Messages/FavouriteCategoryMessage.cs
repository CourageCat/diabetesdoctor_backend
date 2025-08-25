namespace MediaService.Contract.Common.Messages;

public enum FavouriteCategoryMessage
{
    [Message("Cập nhật danh sách thể loại yêu thích thành công.", "favouritecategory_01")]
    UpdateFavouriteCategorySuccessfully,
    [Message("Toàn bộ bài viết từ danh sách yêu thích.", "favouritecategory_02")]
    GetAllPostsFromFavouriteCategorySuccessfully,
}
