using CleanStart.Domain.Entities.Base;
using CleanStart.Domain.Enums;
using CleanStart.Domain.Repositories.Base;
using CleanStart.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CleanStart.Infrastructure.Persistence.Repositories.Base;
public abstract class GenericRepository<TContext, TEntity> : IGenericRepository<TEntity>, IDisposable
    where TContext : DbContext
    where TEntity : class
{
    protected readonly TContext _context;

    protected GenericRepository(TContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetAsync(where, false, includes).ConfigureAwait(false);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> where, bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetQueryable(where, includes, ignoreGlobalQueryFilters).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetSingleAsync(where, false, includes).ConfigureAwait(false);
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> where, bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetQueryable(where, includes, ignoreGlobalQueryFilters).SingleOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<List<TEntity>> GetRowsAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetRowsAsync(where, false, includes).ConfigureAwait(false);
    }

    public async Task<List<TEntity>> GetRowsAsync(Expression<Func<TEntity, bool>> where, bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetQueryable(where, includes, ignoreGlobalQueryFilters).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<TReturn>> GetRowsAsync<TReturn>(Expression<Func<TEntity, bool>> where,
                                                           Expression<Func<TEntity, TReturn>> selector,
                                                           Expression<Func<TEntity, object>> ordeby,
                                                           Expression<Func<TEntity, object>>[] includes) where TReturn : class
    {
        return await GetRowsAsync(where, selector, ordeby, includes, false).ConfigureAwait(false);
    }

    public async Task<List<TReturn>> GetRowsAsync<TReturn>(Expression<Func<TEntity, bool>> where,
                                                           Expression<Func<TEntity, TReturn>> selector,
                                                           Expression<Func<TEntity, object>> ordeby,
                                                           Expression<Func<TEntity, object>>[] includes,
                                                           bool? ignoreGlobalQueryFilters = null) where TReturn : class
    {
        var query = GetQueryable(where, includes, ignoreGlobalQueryFilters).OrderBy(ordeby).Select(selector);
        return await query.ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetAllAsync(false, includes).ConfigureAwait(false);
    }

    public async Task<List<TEntity>> GetAllAsync(bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetQueryable(null, includes, ignoreGlobalQueryFilters).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<TReturn>> GetAllAsync<TReturn>(Expression<Func<TEntity, TReturn>> selector,
                                                          Expression<Func<TReturn, object>> ordeby,
                                                          params Expression<Func<TEntity, object>>[] includes) where TReturn : class
    {
        return await GetAllAsync(selector, ordeby, false, includes).ConfigureAwait(false);
    }

    public async Task<List<TReturn>> GetAllAsync<TReturn>(Expression<Func<TEntity, TReturn>> selector,
                                                          Expression<Func<TReturn, object>> ordeby,
                                                          bool? ignoreGlobalQueryFilters = null,
                                                          params Expression<Func<TEntity, object>>[] includes) where TReturn : class
    {
        var query = GetQueryable(null, includes, ignoreGlobalQueryFilters).Select(selector).OrderBy(ordeby);
        return await query.ToListAsync().ConfigureAwait(false);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? where = null, bool? ignoreGlobalQueryFilters = null)
    {
        return await GetQueryable(where, ignoreGlobalQueryFilters: ignoreGlobalQueryFilters).AnyAsync().ConfigureAwait(false);
    }

    public async Task<PagedList<TEntity>> GetPagedList(PagedListQuery pagedListQuery,
                                                       Expression<Func<TEntity, object>> orderBy,
                                                       Expression<Func<TEntity, bool>>? where = null,
                                                       params Expression<Func<TEntity, object>>[] includes)
    {
        return await GetPagedList(pagedListQuery, orderBy, where, ignoreGlobalQueryFilters: false, includes).ConfigureAwait(false);
    }

    public async Task<PagedList<TEntity>> GetPagedList(PagedListQuery pagedListQuery,
                                                       Expression<Func<TEntity, object>> orderBy,
                                                       Expression<Func<TEntity, bool>>? where = null,
                                                       bool? ignoreGlobalQueryFilters = null,
                                                       params Expression<Func<TEntity, object>>[] includes)
    {
        var filteredQuery = where == null ? GetQueryable(includes: includes, ignoreGlobalQueryFilters: ignoreGlobalQueryFilters) : GetQueryable(where, includes, ignoreGlobalQueryFilters);

        return await GetPagedList(filteredQuery, pagedListQuery, orderBy).ConfigureAwait(false);
    }

    public void Create(TEntity entity, bool trackGraph = false)
    {
        if (trackGraph)
        {
            _context.Set<TEntity>().Add(entity);
        }
        else
        {
            _context.Entry(entity).State = EntityState.Added;
        }
    }

    public void CreateRange(IEnumerable<TEntity> entities, bool trackGraph = false)
    {
        if (trackGraph)
        {
            _context.Set<TEntity>().AddRange(entities);
        }
        else
        {
            foreach (var entity in entities)
            {
                Create(entity);
            }
        }
    }

    public void SoftDelete(TEntity entity, bool trackGraph = false)
    {
        if (entity is ISoftDeleteEntity softDeletable)
        {
            if (trackGraph)
            {
                _context.Set<TEntity>().Update(entity);
            }
            else
            {
                softDeletable.RecordStatus = RecordStatus.Deleted;
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
        else
        {
            throw ObjectNullException.Create("Entity not configured for soft delete");
        }
    }

    public void SoftDeleteRange(IEnumerable<TEntity> entities, bool trackGraph = false)
    {
        foreach (var item in entities)
        {
            SoftDelete(item, trackGraph);
        }
    }

    public void Update(TEntity entity, bool trackGraph = false)
    {
        if (trackGraph)
        {
            _context.Set<TEntity>().Update(entity);
        }
        else
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

    }

    public void UpdateRange(IEnumerable<TEntity> entities, bool trackGraph = false)
    {
        if (trackGraph)
        {
            _context.Set<TEntity>().UpdateRange(entities);
        }
        else
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync().ConfigureAwait(false);
        _context.ChangeTracker?.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _context?.Dispose();
    }


    #region Private Helpers

    private IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>>? @where = null,
                                             Expression<Func<TEntity, object>>[]? includes = null,
                                             bool? ignoreGlobalQueryFilters = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

        if (@where is not null) query = query.Where(@where);

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (ignoreGlobalQueryFilters ?? false) query = query.IgnoreQueryFilters();

        return query;
    }

    private async Task<PagedList<TEntity>> GetPagedList(IQueryable<TEntity> query,
                                                        PagedListQuery pagedListQuery,
                                                        Expression<Func<TEntity, object>> orderBy,
                                                        Expression<Func<TEntity, bool>>? where = null)
    {
        var filteredQuery = where == null ? query : query.Where(where);
        var totalCount = await filteredQuery.CountAsync().ConfigureAwait(false);

        if (pagedListQuery.OrderDirection == OrderDirection.Asc)
        {
            filteredQuery = filteredQuery.OrderBy(orderBy);
        }
        else
        {
            filteredQuery = filteredQuery.OrderByDescending(orderBy);
        }

        var items = await filteredQuery.Skip(pagedListQuery.Page * pagedListQuery.PageSize).Take(pagedListQuery.PageSize).ToListAsync().ConfigureAwait(false);

        var reminder = totalCount % (double)pagedListQuery.PageSize >= 1 ? 1 : 0;
        var totalPages = (totalCount / pagedListQuery.PageSize) + reminder;
        return new PagedList<TEntity>()
        {
            TotalCount = totalCount,
            Data = items,
            CurrentPage = pagedListQuery.Page,
            TotalPages = totalPages
        };
    }


    #endregion

}