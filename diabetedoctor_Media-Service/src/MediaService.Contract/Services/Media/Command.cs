using MediaService.Contract.DTOs.MediaDTOs;
using MongoDB.Bson;

namespace MediaService.Contract.Services.Media;

//public record UploadImagesCommand(List<IFormFile> Images) : ICommand<Success<IEnumerable<MediaResponse>>>;

public class UploadFilesCommand : ICommand<Success<IEnumerable<MediaResponse>>>
{
    public IFormFileCollection Images { get; init; } = default!;
    public string UploadedBy { get; init; } = default!;
}

public class DeleteFilesCommand : ICommand<Success>
{
    public List<string> ImageIds { get; init; } = default!;
}

