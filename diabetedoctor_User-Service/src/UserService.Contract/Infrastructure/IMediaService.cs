using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using MediaType = UserService.Contract.Enums.MediaType;

namespace UserService.Contract.Infrastructure;

public interface IMediaService
{
    Task<UploadResult> UploadFileAsync(IFormFile fileImage, MediaType mediaType);
    Task<DelResResult> DeleteFilesAsync(string[] publicIds);

}
