namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class PatientCreatedProfileCommandHandler
    : ICommandHandler<PatientCreatedProfileCommand, Success>
{
    private readonly IRepositoryBase<User, Guid> _userRepo;
    private readonly IUnitOfWork _unitOfWork;

    public PatientCreatedProfileCommandHandler(IRepositoryBase<User, Guid> userRepo, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(PatientCreatedProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepo.FindSingleAsync(u => u.Id == command.UserId, cancellationToken);
        if (user == null)
            // Saga
            throw new Exception("Error");

        user.UpdateInformation(fullName: command.FullName, avatar: new Image(command.FullName + "_image", command.Avatar));
        user.MarkAsFirstUpdated();
        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new Success("", ""));
    }
}
