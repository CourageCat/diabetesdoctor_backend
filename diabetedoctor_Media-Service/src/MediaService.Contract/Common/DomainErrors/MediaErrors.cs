using MediaService.Contract.Common.Messages;

namespace MediaService.Contract.Common.DomainErrors;

public static class MediaErrors
{
    public static readonly Error ImageNotFoundException = Error.NotFound(MediaMessage.FileNotFoundException.GetMessage().Code,
        MediaMessage.FileNotFoundException.GetMessage().Message);
    public static readonly Error ImagesNotFoundException = Error.NotFound(MediaMessage.FilesNotFoundException.GetMessage().Code,
        MediaMessage.FilesNotFoundException.GetMessage().Message);
    public static readonly Error UploadImagesFailedException = Error.NotFound(MediaMessage.UploadFilesFailedException.GetMessage().Code,
        MediaMessage.UploadFilesFailedException.GetMessage().Message);
}