using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class ServicePackageErrors
{
    public static readonly Error ServicePackageNotFound = Error.NotFound(ServicePackageMessages.ServicePackageNotFound.GetMessage().Code,
        ServicePackageMessages.ServicePackageNotFound.GetMessage().Message);
}