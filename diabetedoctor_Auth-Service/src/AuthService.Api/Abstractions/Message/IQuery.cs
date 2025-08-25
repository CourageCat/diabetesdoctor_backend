namespace AuthService.Api.Abstractions.Message;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}