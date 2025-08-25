using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class UseConsultationSessionCommandHandler : ICommandHandler<UseConsultationSessionCommand, Success>
{
    private readonly IRepositoryBase<UserInfo, Guid>  _userRepository;
    private readonly IUserPackageRepository _userPackageRepository;

    public UseConsultationSessionCommandHandler(IRepositoryBase<UserInfo, Guid> userRepository, IUserPackageRepository userPackageRepository)
    {
        _userRepository = userRepository;
        _userPackageRepository = userPackageRepository;
    }

    public async Task<Result<Success>> Handle(UseConsultationSessionCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userRepository.AnyAsync(user => user.Id == command.UserId, cancellationToken);
        if (!userFound)
        {
            throw new Exception("Không tìm thấy người dùng này!");
        }
        var activePackage = await _userPackageRepository.GetFirstActivePackageAsync(command.UserId, cancellationToken);
        if (activePackage is null)
        {
            throw new Exception("Số lượt tư vấn không đủ!");
        }
        activePackage.ReduceSession();
        return Result.Success(new Success("",""));
    }
}