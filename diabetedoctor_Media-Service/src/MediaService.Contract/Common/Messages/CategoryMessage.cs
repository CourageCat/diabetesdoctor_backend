namespace MediaService.Contract.Common.Messages;

public enum CategoryMessage
{
    [Message("Tên thể loại đã tồn tại!", "category_01")]
    CategoryNameExistException,
    [Message("Tạo thể loại thành công.", "category_02")]
    CreateCategorySuccessfully,
    [Message("Tất cả thể loại:", "category_03")]
    GetAllCategoriesSuccessfully,
    [Message("Tất cả thể loại có số lượng bài viết cao nhất:", "category_04")]
    GetTopPostCategoriesSuccessfully,
    [Message("Chi tiết của thể loại:", "category_05")]
    GetCategoryByIdSuccessfully,
    [Message("Xóa thể loại thành công.", "category_06")]
    DeleteCategorySuccessfully,
    [Message("Cập nhật thể loại thành công.", "category_07")]
    UpdateCategorySuccessfully,

    [Message("Không tìm thấy thể loại!", "category_error_01")]
    CategoryNotFoundException,
    [Message("Không tìm thấy các thể loại đã chọn!", "category_error_02")]
    CategoriesNotFoundException,
}
