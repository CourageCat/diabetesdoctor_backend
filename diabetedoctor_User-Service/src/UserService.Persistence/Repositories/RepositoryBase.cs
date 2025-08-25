using Microsoft.EntityFrameworkCore.Query;

namespace UserService.Persistence.Repositories;

public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
     where TEntity : DomainEntity<TKey>
{

    private readonly ApplicationDbContext _context;

    public RepositoryBase(ApplicationDbContext context)
        => _context = context;

    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking();
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        if (predicate is not null)
            items = items.Where(predicate);

        return items;
    }

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await FindAll(predicate, includeProperties)
            .AsTracking().ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> FindSingleAsync(
    Expression<Func<TEntity, bool>>? predicate = null,
    bool isTracking = true,
    CancellationToken cancellationToken = default,
    params Expression<Func<TEntity, object>>[] includeProperties)
    {
        if (isTracking)
        {
            return await FindAll(predicate, includeProperties)
                .AsTracking()
                .SingleOrDefaultAsync(cancellationToken);
        }
        else
        {
            return await FindAll(predicate, includeProperties)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }

    public async Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = FindAll(predicate, includeProperties).AsTracking().OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate"));;
        

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking();
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        }
        if (predicate != null)
        {
            items = items.Where(predicate);
        }

        return await items.AnyAsync(cancellationToken);
    }

    public void Add(TEntity entity)
    {
        _context.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _context.AddRange(entities);
    }

    public void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public void RemoveMultiple(List<TEntity> entities)
        => _context.Set<TEntity>().RemoveRange(entities);

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }
    public int UpdateMany(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setProperties)
    {
        return _context.Set<TEntity>()
            .Where(predicate)
            .ExecuteUpdate(setProperties);
    }

    public void AddRange(List<TEntity> entities)
    {
        _context.AddRange(entities);
    }
}

