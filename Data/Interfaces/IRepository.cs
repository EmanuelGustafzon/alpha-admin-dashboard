using Data.Models;
using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IRepository<TEntity, TModel> where TEntity : class
{

    public Task<RepositoryResult<TModel>> CreateAsync(TEntity entity);

    public Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] joins);

    public Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>> selector, bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] joins);

    public Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] joins);

    public Task<RepositoryResult<TModel>> UpdateAsync(TEntity entity);

    public Task<RepositoryResult<bool>> DeleteAsync(TEntity entity);

    public Task BeginTransactionAsync();

    public Task CommitTransactionAsync();

    public Task RollbackTransactionAsync();
}
