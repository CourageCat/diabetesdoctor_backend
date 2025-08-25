using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Contract.Services.ServicePackages.Queries;

public sealed class GetServicePackageByIdQuery : IQuery<Success<ServicePackageResponse>>
{
    public Guid ServicePackageId { get; init; }
}