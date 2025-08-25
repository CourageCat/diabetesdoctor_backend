namespace UserService.Contract.Common.Messages;

public enum MediaMessages
{
    [Message("Thêm tệp tin thành công.", "media_01")]
    UploadFilesSuccessfully,
    [Message("Xóa tệp tin thành công.", "media_02")]
    DeleteFilesSuccessfully,
    
    [Message("Không tìm thấy tệp tin!", "media_error_01")]
    FileNotFound,
    [Message("Không tìm thấy bất kì tệp tin nào!", "media_error_02")]
    FilesNotFound,
    [Message("Thêm tệp tin thất bại!", "media_error_03")]
    UploadFilesFailed,
}
