using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MediaService.Contract.DTOs.PostDTOs;
public record ReviewPostRequestDto
{
    public bool IsApproved { get; init; }
    public string? ReasonRejected { get; init; }
}
