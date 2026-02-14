using System.Linq.Expressions;

namespace CleanStart.Domain.Repositories.Base;
public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> where, bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes);


    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> where, bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes);


    Task<List<TEntity>> GetRowsAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
    Task<List<TEntity>> GetRowsAsync(Expression<Func<TEntity, bool>> where, bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes);


    Task<List<TReturn>> GetRowsAsync<TReturn>(Expression<Func<TEntity, bool>> where,
                                              Expression<Func<TEntity, TReturn>> selector,
                                              Expression<Func<TEntity, object>> ordeby,
                                              Expression<Func<TEntity, object>>[] includes) where TReturn : class;

    Task<List<TReturn>> GetRowsAsync<TReturn>(Expression<Func<TEntity, bool>> where,
                                              Expression<Func<TEntity, TReturn>> selector,
                                              Expression<Func<TEntity, object>> ordeby,
                                              Expression<Func<TEntity, object>>[] includes,
                                              bool? ignoreGlobalQueryFilters = null) where TReturn : class;


    Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
    Task<List<TEntity>> GetAllAsync(bool? ignoreGlobalQueryFilters = null, params Expression<Func<TEntity, object>>[] includes);


    Task<List<TReturn>> GetAllAsync<TReturn>(Expression<Func<TEntity, TReturn>> selector,
                                             Expression<Func<TReturn, object>> ordeby,
                                             params Expression<Func<TEntity, object>>[] includes) where TReturn : class;
    Task<List<TReturn>> GetAllAsync<TReturn>(Expression<Func<TEntity, TReturn>> selector,
                                             Expression<Func<TReturn, object>> ordeby,
                                             bool? ignoreGlobalQueryFilters = null,
                                             params Expression<Func<TEntity, object>>[] includes) where TReturn : class;


    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? where = null, bool? ignoreGlobalQueryFilters = null);

    Task<PagedList<TEntity>> GetPagedList(PagedListQuery pagedListQuery,
                                          Expression<Func<TEntity, object>> orderBy,
                                          Expression<Func<TEntity, bool>>? where = null,
                                          params Expression<Func<TEntity, object>>[] includes);

    Task<PagedList<TEntity>> GetPagedList(PagedListQuery pagedListQuery,
                                          Expression<Func<TEntity, object>> orderBy,
                                          Expression<Func<TEntity, bool>>? where = null,
                                          bool? ignoreGlobalQueryFilters = null,
                                          params Expression<Func<TEntity, object>>[] includes);


    void Create(TEntity entity, bool trackGraph = false);
    void CreateRange(IEnumerable<TEntity> entities, bool trackGraph = false);
    void SoftDelete(TEntity entity, bool trackGraph = false);
    void SoftDeleteRange(IEnumerable<TEntity> entities, bool trackGraph = false);
    void Update(TEntity entity, bool trackGraph = false);
    void UpdateRange(IEnumerable<TEntity> entities, bool trackGraph = false);
    Task SaveChangesAsync();

}
