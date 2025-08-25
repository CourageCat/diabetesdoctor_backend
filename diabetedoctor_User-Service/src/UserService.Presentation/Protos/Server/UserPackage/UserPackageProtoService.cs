using Grpc.Core;
using UserService.Contract.Services.Patients.Queries;

namespace UserService.Presentation.Protos.Server.UserPackage;
public class UserPackageProtoService(ISender sender) : UserPackage.UserPackageBase
{
    public override async Task<CheckSessionRemainingResponse> CheckSessionRemaining(CheckSessionRemainingRequest request, ServerCallContext context)
    {
        var userIdHeader = request.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return await Task.FromResult(new CheckSessionRemainingResponse
            {
                IsSuccess = false
            });
        var query = new CheckSessionRemainingQuery()
        {
            UserId = userId
        };
        var result = await sender.Send(query);
        var response = new CheckSessionRemainingResponse
        {
            IsSuccess = result.IsSuccess
        };
        if (result.IsSuccess)
        {
            response.Price = result.Value.Data!.Price;
            response.UserPackageId = result.Value.Data.UserPackageId.ToString();
        }
        return await Task.FromResult(response);
    }
}
