using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class RefundConsultationSessionCommandHandler : ICommandHandler<RefundConsultationSessionCommand>
{
    private readonly IRepositoryBase<UserInfo, Guid> _userRepository;
    private readonly IRepositoryBase<UserPackage, Guid> _userPackageRepository;

    public RefundConsultationSessionCommandHandler(IRepositoryBase<UserInfo, Guid> userRepository, IRepositoryBase<UserPackage, Guid> userPackageRepository)
    {
        _userRepository = userRepository;
        _userPackageRepository = userPackageRepository;
    }
    
    public async Task<Result> Handle(RefundConsultationSessionCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userRepository.AnyAsync(user => user.Id == command.UserId, cancellationToken);
        if (!userFound)
        {
            throw new Exception("Không tìm thấy người dùng này!");
        }  
        var userPackageFound = await _userPackageRepository.FindSingleAsync(up => up.Id == command.UserPackageId, true, cancellationToken);
        if (userPackageFound is null)
        {
            throw new Exception("Không tìm thấy gói dịch vụ của người dùng");
        }
        userPackageFound.IncreaseSession();
        return Result.Success();
    }
}