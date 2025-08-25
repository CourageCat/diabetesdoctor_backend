using ConsultationService.Contract.Exceptions.BusinessExceptions;
using ConsultationService.Contract.Services.Consultation.IntegrationCommands;

namespace ConsultationService.Application.UseCase.V1.IntegrationCommands.Consultations;

public sealed class UpdateConsultationStatusCommandHandler(
    IUnitOfWork unitOfWork,
    IConsultationRepository consultationRepository)
    : ICommandHandler<UpdateConsultationStatusCommand>
{
    public async Task<Result> Handle(UpdateConsultationStatusCommand request, CancellationToken cancellationToken)
    {
        var consultation = await consultationRepository.FindSingleAsync(
            c => c.Id == request.ConsultationId,
            cancellationToken: cancellationToken);
        if (consultation is null)
        {
            throw new ConsultationExceptions.ConsultationNotFoundException();
        }

        switch (request.IsDone)
        {
            case true: consultation.Done();
                break;
            case false: consultation.Start();
                break;
        }

        await consultationRepository.ReplaceOneAsync(unitOfWork.ClientSession, consultation, cancellationToken);
        return Result.Success();
    }
}