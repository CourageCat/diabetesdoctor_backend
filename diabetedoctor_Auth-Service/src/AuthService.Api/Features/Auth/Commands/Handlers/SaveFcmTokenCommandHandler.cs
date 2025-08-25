using AuthService.Api.Common.DomainErrors;
using AuthService.Api.Infrastructures.EventBus.Events;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

public sealed class SaveFcmTokenCommandHandler : ICommandHandler<SaveFcmTokenCommand, Success>
{
    private readonly IRepositoryBase<User, Guid>  _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IRepositoryBase<OutboxEvent, Guid> _outboxEventRepository;

    public SaveFcmTokenCommandHandler(IRepositoryBase<User, Guid> userRepository, IUnitOfWork unitOfWork, IOptions<KafkaSettings> kafkaConfigs, IRepositoryBase<OutboxEvent, Guid> outboxEventRepository)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _kafkaSettings = kafkaConfigs.Value;
        _outboxEventRepository = outboxEventRepository;
    }

    public async Task<Result<Success>> Handle(SaveFcmTokenCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userRepository.FindSingleAsync(user => user.Id == command.UserId, cancellationToken);
        if (userFound is null)
        {
            return FailureFromMessage(AuthErrors.AccountNotExistException);
        }
        userFound.UpdateFcmToken(command.FcmToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        PublishIntegrationEvent(command.UserId, command.FcmToken);
        return Result.Success(new Success(
            AuthMessages.SaveFcmTokenSuccessfully.GetMessage().Code, 
            AuthMessages.SaveFcmTokenSuccessfully.GetMessage().Message));
    }
    
    private void PublishIntegrationEvent(Guid userId, string fcmToken)
    {
        var integrationEvent = new UserInfoFcmTokenUpdatedIntegrationEvent()
        {
            UserId = userId,
            FcmToken = fcmToken
        };
        var outboxEvent =
            OutboxEventExtension.ToOutboxEvent(_kafkaSettings.UserTopic,
                integrationEvent);
        _outboxEventRepository.Add(outboxEvent);
    }
    
    /// <summary>
    /// Tạo kết quả thất bại dựa trên enum AuthMessages.
    /// </summary>
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}