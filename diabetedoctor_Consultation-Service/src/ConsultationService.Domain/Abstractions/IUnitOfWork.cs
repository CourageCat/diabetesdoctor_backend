using MongoDB.Driver;

namespace ConsultationService.Domain.Abstractions;

public interface IUnitOfWork
{
    IClientSessionHandle ClientSession { get; }
    
    Task StartTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task AbortTransactionAsync(CancellationToken cancellationToken = default);
}