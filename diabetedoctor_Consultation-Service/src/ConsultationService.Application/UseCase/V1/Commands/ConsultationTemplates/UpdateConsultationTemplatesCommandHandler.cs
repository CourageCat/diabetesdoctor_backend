using System.Globalization;
using ConsultationService.Contract.Enums;
using ConsultationService.Contract.Helpers;
using ConsultationService.Contract.Services.ConsultationTemplate;
using ConsultationService.Contract.Services.ConsultationTemplate.Commands;
using ConsultationService.Domain.Enums;

namespace ConsultationService.Application.UseCase.V1.Commands.ConsultationTemplates;

public sealed class UpdateConsultationTemplatesCommandHandler (
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IConsultationTemplateRepository consultationTemplateRepository)
    : ICommandHandler<UpdateConsultationTemplateCommand, Response>
{
    public async Task<Result<Response>> Handle(UpdateConsultationTemplateCommand request, CancellationToken cancellationToken)
    {
        var staffId = UserId.Of(request.StaffId);
        var doctorId = UserId.Of(request.DoctorId);
        var permission = await CheckDoctorBelongToHospital(staffId, doctorId, cancellationToken);
        if (permission.IsFailure)
        {
            return Result.Failure<Response>(permission.Error);
        }
        
        var templateToInsert = request.UpsertTimeTemplates.Where(template => template.TimeTemplateId is null).ToList();
        List<ConsultationTemplate> insertedTemplates = [];
        if (templateToInsert.Count > 0)
        {
            // do insert
            var createResult = MapToListConsultationTemplate(doctorId, templateToInsert);
            if (createResult.IsFailure && createResult is IValidationResult validationResult)
            {
                return ValidationResult<Response>.WithErrors(validationResult.Errors);
            }
            insertedTemplates = createResult.Value;
        }

        var templatesToUpdate = request.UpsertTimeTemplates.Where(template => template.TimeTemplateId is not null).ToList();
        List<ConsultationTemplate> updatedTemplates = [];
        if (templatesToUpdate.Count > 0)
        {
            // do update
            var updateResult = await UpdateConsultationTemplates(doctorId, templatesToUpdate, request.Status, cancellationToken);
            if (updateResult.IsFailure)
            {
                return updateResult is IValidationResult validationResult 
                    ? ValidationResult<Response>.WithErrors(validationResult.Errors)
                    : Result.Failure<Response>(updateResult.Error);
            }
            updatedTemplates = updateResult.Value;
        }

        FilterDefinition<ConsultationTemplate>? deleteFilter = null;
        if (request.TemplateIdsToDelete.Any())
        {
            // do delete
            var templateObjectIdsToDelete = request.TemplateIdsToDelete.Select(x => ObjectId.TryParse(x, out var id) ? id : ObjectId.Empty);
            var builder = Builders<ConsultationTemplate>.Filter;
            deleteFilter = builder.And(
                builder.In(ct => ct.Id, templateObjectIdsToDelete),
                builder.Ne(ct => ct.Status, ConsultationTemplateStatus.Booked));
        }
        
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            if (insertedTemplates.Count > 0)
            {
                await consultationTemplateRepository.CreateManyAsync(unitOfWork.ClientSession, insertedTemplates, cancellationToken);
            }

            if (updatedTemplates.Count > 0)
            {
                await consultationTemplateRepository.ReplaceManyAsync(unitOfWork.ClientSession, updatedTemplates, cancellationToken);
            }

            if (deleteFilter is not null)
            {
                await consultationTemplateRepository.DeleteManyAsync(unitOfWork.ClientSession, deleteFilter, cancellationToken);
            }
            
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        
        return Result.Success(new Response(
            ConsultationTemplateMessage.UpdateTemplateSuccessfully.GetMessage().Code,
            ConsultationTemplateMessage.UpdateTemplateSuccessfully.GetMessage().Message));
    }

    private async Task<Result> CheckDoctorBelongToHospital(UserId staffId, UserId doctorId, CancellationToken cancellationToken)
    {
        var staffProjection = Builders<User>.Projection.Include(x => x.HospitalId).Exclude(x => x.Id);
        var staff = await userRepository.FindSingleAsync(
            u => u.UserId == staffId && u.IsDeleted == false && u.HospitalId != null,
            staffProjection,
            cancellationToken);

        if (staff is null)
        {
            return Result.Failure<UserId>(UserErrors.StaffNotFound);
        }
        
        var doctorBelongToHospital = await userRepository.ExistsAsync(
            u => u.UserId == doctorId && u.HospitalId == staff.HospitalId && u.IsDeleted == false,
            cancellationToken);

        return doctorBelongToHospital ? Result.Success() : Result.Failure(UserErrors.DoctorNotBelongToHospital);
    }
    
    private static Result<List<ConsultationTemplate>> MapToListConsultationTemplate(UserId doctorId, IEnumerable<UpsertTimeTemplate> templatesToInsert)
    {
        var consultationTemplates = new List<ConsultationTemplate>();
        foreach (var template in templatesToInsert)
        {
            var consultationTemplate = ConsultationTemplate.Create(
                id: ObjectId.GenerateNewId(),
                doctorId: doctorId,
                date: (DateOnly)template.Date!,
                startTime: (TimeOnly)template.TimeRange.Start!,
                endTime: (TimeOnly)template.TimeRange.End!);
            
            if (consultationTemplate.IsFailure && consultationTemplate is IValidationResult validationResult)
            {
                return ValidationResult<List<ConsultationTemplate>>.WithErrors(validationResult.Errors);
            }
            consultationTemplates.Add(consultationTemplate.Value);
        }
        
        return Result.Success(consultationTemplates);
    }

    private async Task<Result<List<ConsultationTemplate>>> UpdateConsultationTemplates(
        UserId doctorId, List<UpsertTimeTemplate> templatesToUpdate, ConsultationTemplateStatusEnum? statusEnum,
        CancellationToken cancellationToken)
    {
        var updateTemplateIds = templatesToUpdate.Select(t => ObjectId.TryParse(t.TimeTemplateId, out var id) ? id : ObjectId.Empty);
        var templates = (await consultationTemplateRepository.FindListAsync(
            ct => updateTemplateIds.Contains(ct.Id) 
                  && ct.IsDeleted == false
                  && ct.DoctorId == doctorId,
            cancellationToken: cancellationToken)).ToHashSet();
        if (templates.Count != templatesToUpdate.Count)
        {
            return Result.Failure<List<ConsultationTemplate>>(ConsultationTemplateErrors.NotFound);
        }

        foreach (var template in templates)
        {
            if (template.Status is ConsultationTemplateStatus.Booked)
            {
                return Result.Failure<List<ConsultationTemplate>>(ConsultationTemplateErrors.TemplateIsBooked);
            }
        }
        
        foreach (var updateTemplate in templatesToUpdate)
        {
            var template = templates.FirstOrDefault(t => t.Id.ToString() == updateTemplate.TimeTemplateId);
            if (template is null) continue;
            var result = template.Modify(updateTemplate.TimeRange.Start, updateTemplate.TimeRange.End);
            if (result.IsFailure && result is IValidationResult validationResult)
            {
                return ValidationResult<List<ConsultationTemplate>>.WithErrors(validationResult.Errors);
            }
            var status = statusEnum.ToEnumNullable<ConsultationTemplateStatusEnum, ConsultationTemplateStatus>();
            switch (status)
            {
                case ConsultationTemplateStatus.Available:
                    template.Available();       
                    break;
                case ConsultationTemplateStatus.Unavailable:
                    template.Unavailable();
                    break;
            }
        }

        return Result.Success(templates.ToList());
    }
}