using ConsultationService.Contract.Abstractions.Shared;
using MediatR;

namespace ConsultationService.Contract.Abstractions.Message;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}