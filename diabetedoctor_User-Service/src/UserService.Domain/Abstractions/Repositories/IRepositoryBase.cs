using Microsoft.EntityFrameworkCore.Query;

namespace UserService.Domain.Abstractions.Repositories;

public interface IRepositoryBase<TEntity, in TKey> where TEntity : DomainEntity<TKey>
{
    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);
    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, bool isTracking = true, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);
    Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    int UpdateMany(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setProperties);
    void Remove(TEntity entity);
    void RemoveMultiple(List<TEntity> entities);
}