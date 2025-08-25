using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.ServicePackages.Commands;

namespace UserService.Application.UseCases.V1.Commands.ServicePackages;

public sealed class CreateServicePackageCommandHandler : ICommandHandler<CreateServicePackageCommand, Success>
{
    private readonly IRepositoryBase<ServicePackage, Guid> _servicePackageRepository;
    private readonly IRepositoryBase<AdminProfile, Guid> _adminProfileRepository;

    public CreateServicePackageCommandHandler(IRepositoryBase<ServicePackage, Guid> servicePackageRepository, IRepositoryBase<AdminProfile, Guid> adminProfileRepository)
    {
        _servicePackageRepository = servicePackageRepository;
        _adminProfileRepository = adminProfileRepository;
    }

    public async Task<Result<Success>> Handle(CreateServicePackageCommand command, CancellationToken cancellationToken)
    {
        var adminFound = await _adminProfileRepository.FindSingleAsync(admin => admin.UserId == command.AdminId, true, cancellationToken);
        if (adminFound is null)
        {
            return Result.Failure<Success>(AdminErrors.AdminNotFound);
        }

        var servicePackageId = new UuidV7().Value;
        var servicePackageAdded = ServicePackage.Create(servicePackageId, command.Name, command.Description, command.Price, command.Sessions, command.DurationInMonths, adminFound.Id);
        _servicePackageRepository.Add(servicePackageAdded);        
        return Result.Success(new Success(ServicePackageMessages.CreateServicePackageSuccessfully.GetMessage().Code, ServicePackageMessages.CreateServicePackageSuccessfully.GetMessage().Message));
    }
}