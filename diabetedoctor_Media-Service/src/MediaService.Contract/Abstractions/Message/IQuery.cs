using MediaService.Contract.Abstractions.Shared;

namespace MediaService.Contract.Abstractions.Message;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}