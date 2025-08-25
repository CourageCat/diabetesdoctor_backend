using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using Newtonsoft.Json;
using UserService.Contract.Common.DomainErrors;
using UserService.Contract.DTOs.Payment;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Payments.Commands;

namespace UserService.Application.UseCases.V1.Commands.Payments;

public sealed class WebhookCommandHandler : ICommandHandler<WebhookCommand, Success>
{
    private readonly IServicePackageRepository _servicePackageRepository;
    private readonly IRepositoryBase<UserPackage, Guid> _userPackageRepository;
    private readonly IRepositoryBase<PaymentHistory, Guid> _paymentHistoryRepository;
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepository;
    private readonly IPaymentService _paymentService;
    private readonly IResponseCacheService _responseCacheService;
    private readonly ILogger<WebhookCommandHandler> _logger;

    public WebhookCommandHandler(IServicePackageRepository servicePackageRepository,
        IRepositoryBase<UserPackage, Guid> userPackageRepository,
        IRepositoryBase<PaymentHistory, Guid> paymentHistoryRepository,
        IRepositoryBase<UserInfo, Guid> userInfoRepository, IPaymentService paymentService,
        IResponseCacheService responseCacheService, ILogger<WebhookCommandHandler> logger)
    {
        _servicePackageRepository = servicePackageRepository;
        _userPackageRepository = userPackageRepository;
        _paymentHistoryRepository = paymentHistoryRepository;
        _userInfoRepository = userInfoRepository;
        _paymentService = paymentService;
        _responseCacheService = responseCacheService;
        _logger = logger;
    }

    public async Task<Result<Success>> Handle(WebhookCommand command, CancellationToken cancellationToken)
    {
        // Confirm webhook
        var verifySignature = _paymentService.VerifyPayment(command.WebhookType);
        if (!verifySignature)
        {
            return Result.Failure<Success>(PaymentErrors.VerifySignatureFailed);
        }

        try
        {
            var servicePackagePaymentInMemory =
                await _responseCacheService.GetCacheResponseAsync($"order_{command.WebhookType.data.orderCode}");
            
            var servicePackagePayment =
                JsonConvert.DeserializeObject<ResultCacheDto.ServicePackagePaymentCacheDTO>(
                    servicePackagePaymentInMemory!);
            var userFound =
                await _userInfoRepository.AnyAsync(user => user.Id == servicePackagePayment!.UserId,
                    cancellationToken);
            if (!userFound)
            {
                _logger.LogInformation(
                    $"Can not found User with Id: {servicePackagePayment!.UserId}");
                throw new Exception();
            }
            
            var servicePackageFound =
                await _servicePackageRepository.FindSingleAsync(
                    package => package.Id == servicePackagePayment!.ServicePackageId, false, cancellationToken);
            if (servicePackageFound is null)
            {
                _logger.LogInformation(
                    $"Can not found Service Package with Id: {servicePackagePayment!.ServicePackageId}");
                throw new Exception();
            }
            
            var consultationFee =
                await _servicePackageRepository.GetMinimumConsultationFeePerSessionAsync(cancellationToken);
            var expireDate = DateTime.UtcNow.AddMonths(servicePackageFound.DurationInMonths);
            var userPackageId = new UuidV7().Value;
            var userPackage = UserPackage.Create(userPackageId, servicePackageFound.Name, consultationFee,
                servicePackageFound.Sessions, servicePackageFound.Sessions, expireDate,
                servicePackagePayment!.UserId,
                servicePackageFound.Id);
            
            var paymentHistoryId = new UuidV7().Value;
            var paymentHistory =
                PaymentHistory.Create(paymentHistoryId,
                    servicePackageFound.Price, servicePackagePayment!.OrderCode, servicePackagePayment!.UserId,
                    userPackageId);
            _userPackageRepository.Add(userPackage);
            _paymentHistoryRepository.Add(paymentHistory);
            return Result.Success(new Success("", ""));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error attempting to verify payment webhook: {ExMessage}", ex.Message);
            return Result.Success(new Success("", ""));
        }
    }
}