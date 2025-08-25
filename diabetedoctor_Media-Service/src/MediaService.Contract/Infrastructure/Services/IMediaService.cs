using Microsoft.AspNetCore.Http;
using MediaService.Contract.DTOs.MediaDTOs;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.Formatters;
using MediaType = MediaService.Contract.Enumarations.Media.MediaType;

namespace MediaService.Contract.Infrastructure.Services;

public interface IMediaService
{
    Task<UploadResult> UploadFileAsync(IFormFile fileImage, MediaType mediaType);
    Task<DelResResult> DeleteFilesAsync(string[] publicIds);

}
