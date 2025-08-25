using Microsoft.EntityFrameworkCore.Storage;

namespace UserService.Persistence;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IDisposable
{
    private IDbContextTransaction? _transaction;
    public IDbContextTransaction Transaction => 
        _transaction ?? throw new InvalidOperationException("Transaction has not been started.");
    
    public async Task StartTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            return;
        }

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("A transaction has not been started.");
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await AbortTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task AbortTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        context.Dispose();
    }
}