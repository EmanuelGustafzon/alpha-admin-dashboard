using Data.Models;
using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{

    public Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity);

    public Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync(bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] joins);

    public Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>> selector, bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] joins);

    public Task<RepositoryResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] joins);

    public Task<RepositoryResult<TEntity>> UpdateAsync(TEntity entity);

    public Task<RepositoryResult<TEntity>> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity);

    public Task<RepositoryResult<bool>> DeleteAsync(TEntity entity);

    public Task<RepositoryResult<bool>> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    public Task BeginTransactionAsync();

    public Task CommitTransactionAsync();

    public Task RollbackTransactionAsync();
}
