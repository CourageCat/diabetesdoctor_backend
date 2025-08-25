using ConsultationService.Contract.Abstractions.Shared;
using MediatR;

namespace ConsultationService.Contract.Abstractions.Message;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}