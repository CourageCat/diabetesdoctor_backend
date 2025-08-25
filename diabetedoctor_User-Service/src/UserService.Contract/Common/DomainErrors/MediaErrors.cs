using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class MediaErrors
{
    public static readonly Error ImageNotFound = Error.NotFound(MediaMessages.FileNotFound.GetMessage().Code,
        MediaMessages.FileNotFound.GetMessage().Message);
    public static readonly Error ImagesNotFound = Error.NotFound(MediaMessages.FilesNotFound.GetMessage().Code,
        MediaMessages.FilesNotFound.GetMessage().Message);
    public static readonly Error UploadImagesFailed = Error.Conflict(MediaMessages.UploadFilesFailed.GetMessage().Code,
        MediaMessages.UploadFilesFailed.GetMessage().Message);
}