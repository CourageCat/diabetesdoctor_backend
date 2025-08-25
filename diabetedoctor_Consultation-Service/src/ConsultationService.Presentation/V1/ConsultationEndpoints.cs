using Asp.Versioning.Builder;
using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Common.Pagination;
using ConsultationService.Contract.DTOs.ConsultationDtos.Requests;
using ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Requests;
using ConsultationService.Contract.Infrastructure.Services;
using ConsultationService.Contract.Services.Consultation.Commands;
using ConsultationService.Contract.Services.Consultation.Filters;
using ConsultationService.Contract.Services.Consultation.Queries;
using ConsultationService.Contract.Services.ConsultationTemplate.Commands;
using ConsultationService.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ConsultationService.Presentation.V1;

public static class ConsultationEndpoints
{
    public const string ApiName = "consultations";
    private const string BaseUrl = $"/api/v{{version:apiVersion}}/{ApiName}";
    
    public static IVersionedEndpointRouteBuilder MapConsultationApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var consultations = builder.MapGroup(BaseUrl).HasApiVersion(1);
        
        // consultation
        consultations.MapPost("", BookConsultation).RequireAuthorization().WithSummary("book a consultation");
        consultations.MapGet("", GetBookedConsultations).RequireAuthorization().WithSummary("get consultation history");
        consultations.MapPatch("{consultationId}/cancel", CancelConsultation).RequireAuthorization().WithSummary("cancel consultation");
        // consultations.MapPost("/test", HandleTestGrpcAsync);

        return builder;
    }
    
    private static async Task<IResult> BookConsultation(ISender sender, IClaimsService claimsService,
        [FromBody] BookConsultationRequest request)
    {
        var userId = claimsService.GetCurrentUserId;
        var command = new BookConsultationCommand
        {
            TemplateId = request.TemplateId,
            UserId = userId
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> CancelConsultation(ISender sender, IClaimsService claimsService,
        ObjectId consultationId,
        [FromBody] CancelConsultationRequest request)
    {
        var userId = claimsService.GetCurrentUserId;
        var role = claimsService.GetCurrentRole;
        var command = new CancelConsultationCommand()
        {
            UserId = userId,
            Role = role,
            ConsultationId = consultationId,
            Reason = request.Reason
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> GetBookedConsultations(ISender sender, IClaimsService claimsService,
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetConsultationHistoriesFilter filter)
    {
        var userId = claimsService.GetCurrentUserId;
        var role = claimsService.GetCurrentRole;
        var query = new GetConsultationHistoriesQuery
        {
            UserId = userId,
            Role = role,
            Pagination = cursorPaginationRequest,
            Filter = filter
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    // private static async Task<IResult> HandleTestGrpcAsync(ISender sender, Wallet.WalletClient walletClient, string userId)
    // {
    //     var response = walletClient.DecreaseSession(new DecreaseSessionRequest{UserId = userId});
    //     return Results.Ok(response.IsSuccess);
    // }
}