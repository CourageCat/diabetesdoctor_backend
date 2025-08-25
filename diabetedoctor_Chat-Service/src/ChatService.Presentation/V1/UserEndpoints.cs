using System.ComponentModel.DataAnnotations;
using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.ValueObjectDtos;
using ChatService.Contract.Enums;
using ChatService.Contract.EventBus.Abstractions;
using ChatService.Contract.Services.User.Commands;
using ChatService.Contract.Services.User.Filters;
using ChatService.Contract.Services.User.Queries;

namespace ChatService.Presentation.V1;

public static class UserEndpoints
{
    public const string ApiName = "users";
    private const string BaseUrl = $"/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapUserApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var users = builder.MapGroup(BaseUrl).HasApiVersion(1);
        return builder;
    }

    
}