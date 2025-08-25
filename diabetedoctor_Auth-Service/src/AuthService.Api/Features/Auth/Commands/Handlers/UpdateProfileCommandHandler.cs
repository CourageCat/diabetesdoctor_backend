namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, Success>
{
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IRepositoryBase<User, Guid> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Success>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userRepository.FindSingleAsync(user => user.Id == command.UserId, cancellationToken);
        if (userFound is null)
        {
            throw new Exception("User not found");
        }
        var avatar = command.Avatar is not null ? new Image(userFound.FullName + "_avatar", command.Avatar) : null;
        userFound.UpdateInformation(command.FullName, avatar);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(new Success("", ""));
    }
}