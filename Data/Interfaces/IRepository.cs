using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    public Task<IEnumerable<TEntity>> GetAllAsync();
    public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

    public Task<TEntity> CreateAsync(TEntity entity);

    public Task<TEntity> UpdateAsync(TEntity entity, TEntity updatedEntity);

    public Task<bool> DeleteAsync(TEntity entity);

    public Task BeginTransactionAsync();

    public Task CommitTransactionAsync();

    public Task RollbackTransactionAsync();
}
