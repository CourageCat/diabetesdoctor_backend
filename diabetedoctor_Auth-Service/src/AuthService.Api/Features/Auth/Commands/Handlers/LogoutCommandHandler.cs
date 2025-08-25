using AuthService.Api.Infrastructures.EventBus.Events;

namespace AuthService.Api.Features.Auth.Commands.Handlers;

/// <summary>
/// Xử lý quá trình đăng xuất bằng cách xóa refresh token của người dùng khỏi cache.
/// </summary>
public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand, Success>
{
    private readonly IResponseCacheService _cache;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IRepositoryBase<OutboxEvent, Guid> _outboxEventRepository;

    public LogoutCommandHandler(IResponseCacheService cache, IOptions<KafkaSettings> kafkaConfigs, IRepositoryBase<OutboxEvent, Guid> outboxEventRepository)
    {
        _cache = cache;
        _kafkaSettings = kafkaConfigs.Value;
        _outboxEventRepository = outboxEventRepository;
    }

    public async Task<Result<Success>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Tạo khóa cache dựa trên ID người dùng
        var cacheKey = $"{AuthConstants.RefreshTokenCachePrefix}{command.UserId}";

        // Bước 2: Xóa refresh token của người dùng khỏi cache
        await _cache.DeleteCacheResponseAsync(cacheKey);
        
        PublishIntegrationEvent(command.UserId);
        
        // Bước 3: Trả về kết quả thành công
        return Result.Success(new Success(
            AuthMessages.LogoutSuccessful.GetMessage().Code,
            AuthMessages.LogoutSuccessful.GetMessage().Message));
    }
    
    private void PublishIntegrationEvent(Guid userId)
    {
        var integrationEvent = new UserInfoFcmTokenUpdatedIntegrationEvent()
        {
            UserId = userId,
            FcmToken = null
        };
        var outboxEvent =
            OutboxEventExtension.ToOutboxEvent(_kafkaSettings.UserTopic,
                integrationEvent);
        _outboxEventRepository.Add(outboxEvent);
    }
}
