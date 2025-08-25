using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.Domain;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Abstractions.Repositories;
using NotificationService.Persistence;

namespace NotificationService.Persistence.Repositories;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : DomainEntity<ObjectId>
{ 
    protected readonly IMongoCollection<TEntity> DbSet;

    public RepositoryBase(IMongoDbContext context)
    {
        var database = context.Database;
        DbSet = database.GetCollection<TEntity>(typeof(TEntity).Name);
    }

    public async Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public IQueryable<TEntity> FindAll()
    {
        return DbSet.AsQueryable();
    }
    public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity,bool>> filter, ProjectionDefinition<TEntity> projection = default!, CancellationToken cancellationToken = default)
    {
        return projection switch
        {
            default(ProjectionDefinition<TEntity>) => await DbSet.Find(filter).FirstOrDefaultAsync(cancellationToken),
            _ => await DbSet.Find(filter).Project<TEntity>(projection).FirstOrDefaultAsync(cancellationToken: cancellationToken)
        };
    }

    public async Task<TEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter, ProjectionDefinition<TEntity> projection = default!, CancellationToken cancellationToken = default)
    {
        return projection switch
        {
            default(ProjectionDefinition<TEntity>) => await DbSet.Find(filter).ToListAsync(cancellationToken),
            _ => await DbSet.Find(filter).Project<TEntity>(projection).ToListAsync(cancellationToken: cancellationToken)
        };
    }

    public async Task CreateAsync(IClientSessionHandle session, TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertOneAsync(session, entity, cancellationToken: cancellationToken);
    }
    
    public async Task CreateManyAsync(IClientSessionHandle session, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertManyAsync(session, entities, cancellationToken: cancellationToken);
    }

    public async Task ReplaceOneAsync(IClientSessionHandle session, ObjectId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await DbSet.ReplaceOneAsync(session, filter, entity, cancellationToken: cancellationToken);
    }

    public async Task<UpdateResult> UpdateOneAsync(IClientSessionHandle session, ObjectId id, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
        var combineUpdate = Builders<TEntity>.Update.Combine(update, Builders<TEntity>.Update.Set(x => x.ModifiedDate, DateTime.UtcNow));
        return await DbSet.UpdateOneAsync(session, filter, combineUpdate, new UpdateOptions { IsUpsert = false }, cancellationToken);
    }

    public async Task UpdateManyAsync(IClientSessionHandle session, Expression<Func<TEntity, bool>> filterExpression, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Where(filterExpression);
        await DbSet.UpdateManyAsync(session, filter, update, cancellationToken: cancellationToken);
    }

    //public async Task<UpdateResult> AddToSetEach<TValue>(IClientSessionHandle session, TEntity entity, CancellationToken cancellationToken = default)
    //{
    //    var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
        
    //    var updates = entity.Changes.Select(change =>
    //    {
    //        if (change.Value is not IEnumerable values) 
    //        {
    //            return Builders<TEntity>.Update.AddToSet(change.Key, change.Value);
    //        }

    //        var itemsList = values.Cast<TValue>().ToList();
    //        return Builders<TEntity>.Update.AddToSetEach(change.Key, itemsList);
    //    }).ToList();
        
    //    var combineUpdate = Builders<TEntity>.Update.Combine(updates);
        
    //    return await DbSet.UpdateOneAsync(session,
    //        filter,
    //        combineUpdate,
    //        new UpdateOptions { IsUpsert = false }, cancellationToken);
    //}

    public async Task DeleteAsync(IClientSessionHandle session, ObjectId id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await DbSet.DeleteOneAsync(session, filter, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(filter).AnyAsync(cancellationToken);
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

}