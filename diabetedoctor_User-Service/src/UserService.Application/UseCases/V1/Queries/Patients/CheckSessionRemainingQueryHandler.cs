using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Queries;
using UserService.Contract.Services.Patients.Responses;

namespace UserService.Application.UseCases.V1.Queries.Patients;

public sealed class CheckSessionRemainingQueryHandler(ApplicationDbContext context) : IQueryHandler<CheckSessionRemainingQuery, Success<CheckSessionRemainingResponse>>
{
    public async Task<Result<Success<CheckSessionRemainingResponse>>> Handle(CheckSessionRemainingQuery query, CancellationToken cancellationToken)
    {
        var userFound = await context.UserInfos.AnyAsync(user => user.Id == query.UserId, cancellationToken);
        if (!userFound)
        {
            return Result.Failure<Success<CheckSessionRemainingResponse>>(PatientErrors.ProfileNotExist);
        }
        var firstActivePackage = await context.UserPackages
            .Where(up => !up.IsExpired && up.RemainingSessions > 0 && up.UserId == query.UserId)
            .OrderBy(up => up.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstActivePackage is null)
        {
            return Result.Failure<Success<CheckSessionRemainingResponse>>(PatientErrors.ConsultationSessionsNotEnough);
        }

        var result = new CheckSessionRemainingResponse()
        {
            Price = firstActivePackage.ConsultationFee,
            UserPackageId = firstActivePackage.Id,
        };
        return Result.Success(new Success<CheckSessionRemainingResponse>("","",  result));
    }
}