using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Data.Interfaces;
using Data.Context;
using Data.Models;
using Domain.Extensions;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity>(ApplicationDbContext context) : IRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<TEntity> _table = context.Set<TEntity>();
    private IDbContextTransaction _transaction = null!;

    public virtual async Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity)
    {
        if (entity == null) return RepositoryResult<TEntity>.BadRequest($"No {nameof(entity)} was provided");

        try
        {
            await _table.AddAsync(entity);
            await _context.SaveChangesAsync();

            return RepositoryResult<TEntity>.Created(entity);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating {nameof(TEntity)} entity :: {ex.Message}");
            return RepositoryResult<TEntity>.Errror($"Failed to create{nameof(TEntity)}");
        }
    }

    public virtual async Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync(bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] joins)
    {
        IQueryable<TEntity> query = _table;
        if(filterBy != null)
            query = query.Where(filterBy);

        if(joins != null && joins.Length != 0) 
            foreach(var join in joins)
                query = query.Include(join);

        if(sortBy != null)
            query = orderByDecending 
                ? query.OrderByDescending(sortBy)
                : query.OrderBy(sortBy);

        var entityList = await query.ToListAsync();
        return RepositoryResult<IEnumerable<TEntity>>.Ok(entityList);

    }
    public virtual async Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>> selector, bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] joins)
    {
        IQueryable<TEntity> query = _table;
        if (filterBy != null)
            query = query.Where(filterBy);

        if (joins != null && joins.Length != 0)
            foreach (var join in joins)
                query = query.Include(join);

        if (sortBy != null)
            query = orderByDecending
                ? query.OrderByDescending(sortBy)
                : query.OrderBy(sortBy);

        var entityList = await query.Select(selector).ToListAsync();
        return RepositoryResult<IEnumerable<TSelect>>.Ok(entityList);
    }

    public virtual async Task<RepositoryResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] joins)
    {
        if (findBy == null) return RepositoryResult<TEntity>.BadRequest("No function to find entity by was provided");

        IQueryable<TEntity> query = _table;

        if (joins != null && joins.Length != 0)
            foreach (var join in joins)
                query = query.Include(join);

        var entity = await query.FirstOrDefaultAsync(findBy);

        return entity != null 
            ? RepositoryResult<TEntity>.Ok(entity) 
            : RepositoryResult<TEntity>.NotFound($"{nameof(TEntity)} not found");
    }

    public virtual async Task<RepositoryResult<TEntity>> UpdateAsync(TEntity entity)
    {
        if (entity == null) return RepositoryResult<TEntity>.BadRequest($"No {nameof(entity)} was provided");
        try
        {
            _table.Update(entity);
            await _context.SaveChangesAsync();

            return RepositoryResult<TEntity>.Ok(entity);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error Updating {nameof(TEntity)} entity: {ex.Message}");
            return RepositoryResult<TEntity>.Errror($"Failed to update the {nameof(TEntity)}");
        }
    }

    public virtual async Task<RepositoryResult<bool>> DeleteAsync(TEntity entity)
    {
        if (entity == null) return RepositoryResult<bool>.BadRequest($"No {nameof(entity)} was provided");
        try
        {

            _table.Remove(entity);
            await _context.SaveChangesAsync();
            return RepositoryResult<bool>.NoContent();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error Deleting {nameof(TEntity)} entity :: {ex.Message}");
            return RepositoryResult<bool>.Errror($"Failed to delete the {nameof(TEntity)}");
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
