using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.ServicePackages.Queries;
using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Application.UseCases.V1.Queries.ServicePackages;

public sealed class GetServicePackageByIdQueryHandler(ApplicationDbContext context) : IQueryHandler<GetServicePackageByIdQuery, Success<ServicePackageResponse>>
{
    public async Task<Result<Success<ServicePackageResponse>>> Handle(GetServicePackageByIdQuery request, CancellationToken cancellationToken)
    {
        var servicePackageFound = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.ServicePackageId,  cancellationToken);
        if (servicePackageFound is null)
        {
            return Result.Failure<Success<ServicePackageResponse>>(ServicePackageErrors.ServicePackageNotFound);
        }
        var response = new ServicePackageResponse()
        {
            Id = servicePackageFound.Id.ToString(),
            Name = servicePackageFound.Name,
            Description = servicePackageFound.Description,
            Price = servicePackageFound.Price,
            Sessions = servicePackageFound.Sessions,
            DurationInMonths = servicePackageFound.DurationInMonths,
            CreatedDate = servicePackageFound.CreatedDate!.Value,
        };
        return Result.Success(new Success<ServicePackageResponse>(ServicePackageMessages.GetServicePackageByIdSuccessfully.GetMessage().Code, ServicePackageMessages.GetServicePackageByIdSuccessfully.GetMessage().Message, response));
    }
}