using ConsultationService.Contract.Services.ConsultationTemplate.Commands;

namespace ConsultationService.Application.UseCase.V1.Commands.ConsultationTemplates;

public sealed class DeleteConsultationTemplatesCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IConsultationTemplateRepository consultationTemplateRepository)
    : ICommandHandler<DeleteConsultationTemplatesCommand, Response>
{
    public async Task<Result<Response>> Handle(DeleteConsultationTemplatesCommand request, CancellationToken cancellationToken)
    {
        var staffId = UserId.Of(request.StaffId);
        var doctorId = UserId.Of(request.DoctorId);
        var permission = await CheckDoctorBelongToHospital(staffId, doctorId, cancellationToken);
        if (permission.IsFailure)
        {
            return Result.Failure<Response>(permission.Error);
        }

        var templateIds = request.TemplateIds.Select(ObjectId.Parse).ToList();
        var areExisted = await CheckTemplatesExisted(doctorId, templateIds, cancellationToken);
        if (areExisted.IsFailure)
        {
            return Result.Failure<Response>(areExisted.Error);
        }

        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            var filter = Builders<ConsultationTemplate>.Filter.In(ct => ct.Id, templateIds);
            await consultationTemplateRepository.DeleteManyAsync(unitOfWork.ClientSession, filter, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        
        return Result.Success(new Response(
            ConsultationTemplateMessage.DeleteTemplateSuccessfully.GetMessage().Code,
            ConsultationTemplateMessage.DeleteTemplateSuccessfully.GetMessage().Code));
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
    
    private async Task<Result> CheckTemplatesExisted(UserId doctorId, List<ObjectId> templateIds,
        CancellationToken cancellationToken)
    {
        var project = Builders<ConsultationTemplate>.Projection.Include(ct => ct.Id);
        var templates = await consultationTemplateRepository.FindListAsync(
            ct => templateIds.Contains(ct.Id) 
                  && ct.IsDeleted == false
                  && ct.DoctorId == doctorId,
            project,
            cancellationToken: cancellationToken);
        
        return templates.Count == templateIds.Count ? Result.Success() : Result.Failure(ConsultationTemplateErrors.NotFound);
    }
}