using ConsultationService.Contract.Abstractions.Shared;
using MediatR;

namespace ConsultationService.Contract.Abstractions.Message;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}