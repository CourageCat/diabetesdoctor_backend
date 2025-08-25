using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class AdminErrors
{
    public static readonly Error AdminNotFound = Error.NotFound(AdminMessages.AdminNotFound.GetMessage().Code,
        AdminMessages.AdminNotFound.GetMessage().Message);
}