using Asp.Versioning.Builder;
using ConsultationService.Contract.Common.Pagination;
using ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Requests;
using ConsultationService.Contract.Infrastructure.Services;
using ConsultationService.Contract.Services.ConsultationTemplate.Commands;
using ConsultationService.Contract.Services.ConsultationTemplate.Filters;
using ConsultationService.Contract.Services.ConsultationTemplate.Queries;
using ConsultationService.Domain.Enums;
using ConsultationService.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ConsultationService.Presentation.V1;

public static class DoctorEndpoints
{
    public const string ApiName = "doctors";
    private const string BaseUrl = $"/api/v{{version:apiVersion}}/{ApiName}";
    
    public static IVersionedEndpointRouteBuilder MapDoctorApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var doctors = builder.MapGroup(BaseUrl).HasApiVersion(1);
        
        // consultation-templates
        doctors.MapPost("{doctorId}/consultation-templates", CreateConsultationTemplates).RequireAuthorization()
            .WithSummary("tạo các khung mẫu");
        doctors.MapPatch("{doctorId}/consultation-templates", UpdateConsultationTemplates).RequireAuthorization()
            .WithSummary("Cập nhật khung mẫu");
        doctors.MapGet("{doctorId}/consultation-templates", GetDoctorConsultationTemplates).RequireAuthorization()
            .WithSummary("Lấy toàn bộ khung giờ tư vấn của bác sĩ");
        
        return builder;
    }

    private static async Task<IResult> CreateConsultationTemplates(ISender sender, IClaimsService claimsService,
        string doctorId,
        [FromBody] CreateConsultationTemplateRequest request)
    {
        var staffId = claimsService.GetCurrentUserId;
        var command = new CreateConsultationTemplatesCommand
        {
            StaffId = staffId,
            DoctorId = doctorId,
            TimeTemplates = request.TimeTemplates
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> UpdateConsultationTemplates(ISender sender, IClaimsService claimsService,
        string doctorId,
        [FromBody] UpdateConsultationTemplatesRequest request)
    {
        var staffId = claimsService.GetCurrentUserId;
        var command = new UpdateConsultationTemplateCommand()
        {
            StaffId = staffId,
            DoctorId = doctorId,
            Status = request.Status,
            UpsertTimeTemplates = request.UpsertTimeTemplates,
            TemplateIdsToDelete = request.TemplateIdsToDelete,
        };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> GetDoctorConsultationTemplates(ISender sender, IClaimsService claimsService,
        string doctorId,
        [AsParameters] GetDoctorConsultationTemplatesFilter filter)
    {
        var userId = claimsService.GetCurrentUserId;
        var role = claimsService.GetCurrentRole;
        var query = new GetDoctorConsultationTemplatesQuery()
        {
            UserId = userId,
            DoctorId = doctorId,
            Role = role,
            Filter = filter,
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}