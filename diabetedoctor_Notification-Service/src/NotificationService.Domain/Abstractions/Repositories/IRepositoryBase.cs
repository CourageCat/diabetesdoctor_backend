using MongoDB.Driver;
using System.Linq.Expressions;

namespace NotificationService.Domain.Abstractions.Repositories;

public interface IRepositoryBase<TEntity>
{
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default); 
    Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    IQueryable<TEntity> FindAll();
    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> filter,ProjectionDefinition<TEntity> projection = default!, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter, ProjectionDefinition<TEntity> projection = default!,  CancellationToken cancellationToken = default);
    Task CreateAsync(IClientSessionHandle session, TEntity entity, CancellationToken cancellationToken = default);
    Task ReplaceOneAsync(IClientSessionHandle session, ObjectId id, TEntity entity, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateOneAsync(IClientSessionHandle session, ObjectId id, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default);
    //Task<UpdateResult> AddToSetEach<TValue>(IClientSessionHandle session, TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(IClientSessionHandle session, ObjectId id, CancellationToken cancellationToken = default);
    Task CreateManyAsync(IClientSessionHandle session, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task UpdateManyAsync(IClientSessionHandle session, Expression<Func<TEntity, bool>> filterExpression, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default);

}