using ConsultationService.Contract.Abstractions.Shared;
using MediatR;

namespace ConsultationService.Contract.Abstractions.Message;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
