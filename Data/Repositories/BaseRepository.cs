using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Data.Interfaces;
using Data.Context;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity>(ApplicationDbContext context) : IRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private IDbContextTransaction _transaction = null!;

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity == null) return null!;

        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating {nameof(TEntity)} entity :: {ex.Message}");
            return null!;
        }
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {

        return await _dbSet.ToListAsync();

    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null) return null!;
        return await _dbSet.FirstOrDefaultAsync(predicate) ?? null!;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, TEntity updatedEntity)
    {
        try
        {
            _context.Entry(entity).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync();

            return updatedEntity;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error Updating {nameof(TEntity)} entity: {ex.Message}");
            return null!;
        }
    }

    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        try
        {

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error Deleting {nameof(TEntity)} entity :: {ex.Message}");
            return false;
        }
    }

    // Transactions
    public virtual async Task BeginTransactionAsync()
    {
        _transaction ??= await _context.Database.BeginTransactionAsync();
    }
    public virtual async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }
    }

    public virtual async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }
    }
}
