using UserService.Contract.Services.Doctors.Commands;

namespace UserService.Application.UseCases.V1.Commands.Doctors;

public class ReceiveMoneyFromConsultationCommandHandler : ICommandHandler<ReceiveMoneyFromConsultationCommand>
{
    private readonly IRepositoryBase<UserInfo, Guid> _userRepository;
    private readonly IRepositoryBase<Wallet, Guid> _walletRepository;

    public ReceiveMoneyFromConsultationCommandHandler(IRepositoryBase<UserInfo, Guid> userRepository, IRepositoryBase<Wallet, Guid> walletRepository)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
    }

    public async Task<Result> Handle(ReceiveMoneyFromConsultationCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userRepository.AnyAsync(user => user.Id == command.UserId, cancellationToken);
        if (!userFound)
        {
            throw new Exception("Không tìm thấy người dùng này!");
        }
        var walletFound = await _walletRepository.FindSingleAsync(w => w.UserId == command.UserId, true, cancellationToken);
        if (walletFound is null)
        {
            var walletAdded = Wallet.Create(command.ConsultationFee, command.UserId);
            _walletRepository.Add(walletAdded);
        }
        else
        {
            walletFound.Update(command.ConsultationFee);
        }
        return Result.Success(new Success("",""));
    }
}