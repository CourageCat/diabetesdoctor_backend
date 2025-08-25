using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MediaService.Contract.DTOs.PostDTOs;
public record UpdatePostRequestDto
{
    public string? Title { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    public string? ContentHtml { get; set; } = string.Empty;
    public string? Thumbnail { get; set; } = default!;
    public List<string>? CategoryIds { get; set; } = default!;
    public List<string>? Images { get; set; } = default!;
    public string? DoctorId { get; set; } = string.Empty;
    public bool IsDraft { get; set; }
}
