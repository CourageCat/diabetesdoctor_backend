using System.Globalization;
using ConsultationService.Contract.DTOs.ConsultationTemplateDtos;
using ConsultationService.Contract.Services.ConsultationTemplate;
using ConsultationService.Contract.Services.ConsultationTemplate.Commands;

namespace ConsultationService.Application.UseCase.V1.Commands.ConsultationTemplates;

public sealed class CreateConsultationTemplatesCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IConsultationTemplateRepository consultationTemplateRepository)
    : ICommandHandler<CreateConsultationTemplatesCommand, Response>
{
    public async Task<Result<Response>> Handle(CreateConsultationTemplatesCommand request, CancellationToken cancellationToken)
    {
        var staffId = UserId.Of(request.StaffId);
        var doctorId = UserId.Of(request.DoctorId);
        var check = await CheckDoctorBelongToHospital(staffId, doctorId, cancellationToken);
        if (check.IsFailure)
        {
            return Result.Failure<Response>(check.Error);
        }
        
        var consultationTemplates = MapToListConsultationTemplate(doctorId, request.TimeTemplates);
        if (consultationTemplates.IsFailure && consultationTemplates is IValidationResult validationResult)
        {
            return ValidationResult<Response>.WithErrors(validationResult.Errors);
        }
    
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            await consultationTemplateRepository.CreateManyAsync(unitOfWork.ClientSession, consultationTemplates.Value,
                cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
    
        return Result.Success(new Response(
            ConsultationTemplateMessage.CreateTemplatesSuccessfully.GetMessage().Code,
            ConsultationTemplateMessage.CreateTemplatesSuccessfully.GetMessage().Message));
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

    private static Result<List<ConsultationTemplate>> MapToListConsultationTemplate(UserId doctorId, IEnumerable<TimeTemplate> templates)
    {
        var consultationTemplates = new List<ConsultationTemplate>();
        foreach (var template in templates)
        {
            foreach (var timeRange in template.Times)
            {
                var consultationTemplate = ConsultationTemplate.Create(
                    id: ObjectId.GenerateNewId(),
                    doctorId: doctorId,
                    date: template.Date,
                    startTime: timeRange.Start,
                    endTime: timeRange.End);
                
                if (consultationTemplate.IsFailure && consultationTemplate is IValidationResult validationResult)
                {
                    return ValidationResult<List<ConsultationTemplate>>.WithErrors(validationResult.Errors);
                }
                consultationTemplates.Add(consultationTemplate.Value);
            }
        }
        
        return Result.Success(consultationTemplates);
    }
}