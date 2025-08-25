using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MediaService.Domain.Abstractions.Repositories;

public interface IRepositoryBase<TEntity>
{
    Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TEntity> FindByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    Task CreateAsync(IClientSessionHandle clientSession, TEntity entity, CancellationToken cancellationToken = default);
    Task CreateManyAsync(IClientSessionHandle clientSession, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(IClientSessionHandle clientSession, string id, TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(IClientSessionHandle clientSession, ObjectId id, TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateOneAsync(IClientSessionHandle clientSession, ObjectId id, UpdateDefinition<TEntity> updateDefinition,
        CancellationToken cancellationToken = default);
    Task UpdateManyAsync(IClientSessionHandle clientSession, Expression<Func<TEntity, bool>> filterExpression, UpdateDefinition<TEntity> updateDefinition,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(IClientSessionHandle clientSession, string id, CancellationToken cancellationToken = default);
    Task DeleteAsync(IClientSessionHandle clientSession, ObjectId id, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IClientSessionHandle clientSession, IEnumerable<ObjectId> ids, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(IClientSessionHandle clientSession, CancellationToken cancellationToken = default);
}