using Microsoft.Extensions.Options;
using UserService.Contract.Common.DomainErrors;
using UserService.Contract.DTOs.Payment;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Payments.Commands;
using UserService.Contract.Settings;

namespace UserService.Application.UseCases.V1.Commands.Payments;

public sealed class CreatePaymentBankingCommandHandler : ICommandHandler<CreatePaymentBankingCommand, Success<CreatePaymentResponseDto>>
{
    private readonly IRepositoryBase<ServicePackage, Guid> _servicePackageRepository;
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepository;
    private readonly IPaymentService  _paymentService;
    private readonly PayOSSettings _payOsSettings;
    private readonly IResponseCacheService _responseCacheService;

    public CreatePaymentBankingCommandHandler(IRepositoryBase<ServicePackage, Guid> servicePackageRepository, IRepositoryBase<UserInfo, Guid> userInfoRepository, IPaymentService paymentService, IOptions<PayOSSettings> payOsConfig, IResponseCacheService responseCacheService)
    {
        _servicePackageRepository = servicePackageRepository;
        _userInfoRepository = userInfoRepository;
        _paymentService = paymentService;
        _payOsSettings = payOsConfig.Value;
        _responseCacheService = responseCacheService;
    }

    public async Task<Result<Success<CreatePaymentResponseDto>>> Handle(CreatePaymentBankingCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userInfoRepository.AnyAsync(user => user.Id == command.UserId, cancellationToken);
        if (!userFound)
        {
            return Result.Failure<Success<CreatePaymentResponseDto>>(PatientErrors.ProfileNotExist);
        }

        var servicePackageFound =
            await _servicePackageRepository.FindSingleAsync(package => package.Id == command.ServicePackageId, true, cancellationToken);
        if (servicePackageFound is null)
        {
            return Result.Failure<Success<CreatePaymentResponseDto>>(ServicePackageErrors.ServicePackageNotFound);
        }
        long orderId = new Random().Next(1, 100000);
        // Create payment dto
        List<ItemDto> itemDtos = new()
        {
            new ItemDto(servicePackageFound.Name, 1, (int)servicePackageFound.Price)
        };
        var createPaymentDto = new CreatePaymentDto(orderId, $"Thanh toán", itemDtos, _payOsSettings.ErrorUrl, _payOsSettings.SuccessUrl + $"?orderId={orderId}");
        var result = await _paymentService.CreatePaymentLink(createPaymentDto);
        var resultForCache = new ResultCacheDto.ServicePackagePaymentCacheDTO(result.OrderCode, command.UserId, result.Description, command.ServicePackageId);
        // Save memory to when success or fail will know value
        await _responseCacheService.SetCacheResponseAsync($"order_{orderId}", resultForCache, TimeSpan.FromMinutes(60));

        return Result.Success(new Success<CreatePaymentResponseDto>(PaymentMessages.CreatePaymentBankingSuccessfully.GetMessage().Code, PaymentMessages.CreatePaymentBankingSuccessfully.GetMessage().Message, result));
    }
}