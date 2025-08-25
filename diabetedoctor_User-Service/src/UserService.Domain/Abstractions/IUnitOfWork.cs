using Microsoft.EntityFrameworkCore.Storage;

namespace UserService.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IDbContextTransaction Transaction { get; }
    
    Task StartTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task AbortTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}