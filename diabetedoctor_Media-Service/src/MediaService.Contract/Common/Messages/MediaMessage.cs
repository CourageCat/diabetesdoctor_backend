namespace MediaService.Contract.Common.Messages;

public enum MediaMessage
{
    [Message("Thêm tệp tin thành công.", "media_01")]
    UploadFilesSuccessfully,
    [Message("Xóa tệp tin thành công.", "media_02")]
    DeleteFilesSuccessfully,
    
    [Message("Không tìm thấy tệp tin!", "media_error_01")]
    FileNotFoundException,
    [Message("Không tìm thấy bất kì tệp tin!", "media_error_02")]
    FilesNotFoundException,
    [Message("Thêm tệp tin thất bại!", "media_error_03")]
    UploadFilesFailedException,
}
