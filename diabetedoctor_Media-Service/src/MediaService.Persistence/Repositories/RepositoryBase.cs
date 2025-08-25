using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;

namespace MediaService.Persistence.Repositories;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
{
    protected readonly IMongoCollection<TEntity> DbSet;

    public RepositoryBase(IMongoDbContext context)
    {
        var database = context.Database;
        DbSet = database.GetCollection<TEntity>(typeof(TEntity).Name);
    }

    public async Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountDocumentsAsync(filter, null, cancellationToken);
    }

    public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(Builders<TEntity>.Filter.Eq("_id", id)).FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<TEntity> FindByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(Builders<TEntity>.Filter.Eq("_id", id)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(filter).ToListAsync(cancellationToken);
    }
    
    public async Task CreateAsync(IClientSessionHandle clientSession, TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertOneAsync(clientSession, entity, null, cancellationToken);
    }

    public async Task CreateManyAsync(IClientSessionHandle clientSession, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertManyAsync(clientSession, entities, null, cancellationToken);
    }

    public async Task UpdateAsync(IClientSessionHandle clientSession, string id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        ReplaceOptions updateOptions = null;
        await DbSet.ReplaceOneAsync(clientSession, filter, entity, updateOptions, cancellationToken);
    }

    public async Task UpdateAsync(IClientSessionHandle clientSession, ObjectId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        ReplaceOptions updateOptions = null;
        await DbSet.ReplaceOneAsync(clientSession, filter, entity, updateOptions, cancellationToken);
    }
    
    public async Task UpdateOneAsync(IClientSessionHandle clientSession, ObjectId id, UpdateDefinition<TEntity> updateDefinition, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await DbSet.UpdateOneAsync(clientSession, filter, updateDefinition, cancellationToken: cancellationToken);
    }

    public async Task UpdateManyAsync(IClientSessionHandle clientSession, Expression<Func<TEntity, bool>> filterExpression, UpdateDefinition<TEntity> updateDefinition,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Where(filterExpression);
        await DbSet.UpdateManyAsync(clientSession, filter, updateDefinition, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(IClientSessionHandle clientSession, string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await DbSet.DeleteOneAsync(clientSession, filter, null, cancellationToken);
    }
    public async Task DeleteManyAsync(IClientSessionHandle clientSession, IEnumerable<ObjectId> ids, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.In("_id", ids);
        await DbSet.DeleteManyAsync(clientSession, filter, null, cancellationToken);
    }

    public async Task DeleteAsync(IClientSessionHandle clientSession, ObjectId id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await DbSet.DeleteOneAsync(clientSession, filter, null, cancellationToken);
    }
    public async Task DeleteAllAsync(IClientSessionHandle clientSession, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Empty;
        await DbSet.DeleteManyAsync(clientSession, filter, null, cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(filter).AnyAsync(cancellationToken);
    }
}