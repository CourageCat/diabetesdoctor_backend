using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UserService.Contract.Enums;
using UserService.Contract.Infrastructure;

namespace UserService.Infrastructure.Services;

public class CloudinaryService : IMediaService
{
    private readonly CloudinarySettings _cloudinarySettings;
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig)
    {
        var account = new Account(cloudinaryConfig.Value.CloudName,
            cloudinaryConfig.Value.ApiKey,
            cloudinaryConfig.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
        _cloudinarySettings = cloudinaryConfig.Value;
    }

    public async Task<DelResResult> DeleteFilesAsync(string[] publicIds)
    {
        var result = await _cloudinary.DeleteResourcesAsync(publicIds);
        return result;
    }

    public async Task<UploadResult> UploadFileAsync(IFormFile fileImage, MediaType mediaType)
    {
        var fileName = fileImage.FileName;
        var fileExtension = Path.GetExtension(fileName); // Get the file extension
        var fileBaseName = Path.GetFileNameWithoutExtension(fileName); // Get filename without extension
        if (mediaType == MediaType.Image)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileImage.OpenReadStream()),
                Folder = _cloudinarySettings.Folder,
                Overwrite = true,
            };
            return await _cloudinary.UploadAsync(uploadParams);
        }
        else
        {
            var safeFileName = $"{fileBaseName.Replace(" ", "-")}{fileExtension}"; // Replace spaces with hyphens
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, fileImage.OpenReadStream()),
                Folder = _cloudinarySettings.Folder,
                Overwrite = true,
                PublicId = safeFileName
            };
            return await _cloudinary.UploadAsync(uploadParams);
        }
    }
}