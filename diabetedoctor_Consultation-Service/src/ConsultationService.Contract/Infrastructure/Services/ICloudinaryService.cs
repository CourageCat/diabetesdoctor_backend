using CloudinaryDotNet.Actions;
using ConsultationService.Contract.Enums;
using Microsoft.AspNetCore.Http;

namespace ConsultationService.Contract.Infrastructure.Services;

public interface ICloudinaryService
{
    Task<RawUploadResult> UploadAsync(string id, MediaTypeEnum type, IFormFile formFile, CancellationToken cancellationToken = default);
    Task<DeletionResult?> DeleteAsync(string? publicId);
}