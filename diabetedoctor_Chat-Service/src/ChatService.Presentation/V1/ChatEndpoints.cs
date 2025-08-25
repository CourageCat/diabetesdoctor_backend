using System.ComponentModel.DataAnnotations;
using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.MessageDtos;
using ChatService.Contract.Services.Message.Commands;
using ChatService.Contract.Services.Message.Queries;

namespace ChatService.Presentation.V1;

public static class ChatEndpoints
{
    public const string ApiName = "messages";
    private const string BaseUrl = $"/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapChatApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var messages = builder.MapGroup(BaseUrl).HasApiVersion(1).DisableAntiforgery();
        
        return builder;
    }
}