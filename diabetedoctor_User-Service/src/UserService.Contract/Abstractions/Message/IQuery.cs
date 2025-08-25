namespace UserService.Contract.Abstractions.Message;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}