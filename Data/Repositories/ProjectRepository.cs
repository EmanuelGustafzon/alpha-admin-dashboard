using Data.Context;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class ProjectRepository(ApplicationDbContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{
    private readonly ApplicationDbContext _context = context;

    public override async Task<RepositoryResult<IEnumerable<ProjectEntity>>> GetAllAsync(
       bool orderByDecending = false,
       Expression<Func<ProjectEntity, object>>? sortBy = null,
       Expression<Func<ProjectEntity, bool>>? filterBy = null,
       params Expression<Func<ProjectEntity, object>>[] joins)
    {
        IQueryable<ProjectEntity> query = _context.Set<ProjectEntity>();

        if (filterBy != null)
            query = query.Where(filterBy);

        if (joins != null && joins.Length > 0)
        {
            foreach (var join in joins)
            {
                query = query.Include(join);
            }
        }

        query = query.Include(x => x.MemberProjects)  
                     .ThenInclude(mp => mp.Member); 

        if (sortBy != null)
        {
            query = orderByDecending
                ? query.OrderByDescending(sortBy)
                : query.OrderBy(sortBy);
        }

        var entityList = await query.ToListAsync();

        return RepositoryResult<IEnumerable<ProjectEntity>>.Ok(entityList);
    }

    public override async Task<RepositoryResult<ProjectEntity>> GetAsync(Expression<Func<ProjectEntity, bool>> findBy, params Expression<Func<ProjectEntity, object>>[] joins)
    {
        if (findBy == null) return RepositoryResult<ProjectEntity>.BadRequest("No function to find entity by was provided");

        IQueryable<ProjectEntity> query = _context.Set<ProjectEntity>();

        if (joins != null && joins.Length != 0)
            foreach (var join in joins)
                query = query.Include(join);

        query = query.Include(x => x.MemberProjects)
                     .ThenInclude(mp => mp.Member);

        var entity = await query.FirstOrDefaultAsync(findBy);

        return entity != null
            ? RepositoryResult<ProjectEntity>.Ok(entity)
            : RepositoryResult<ProjectEntity>.NotFound($"{nameof(ProjectEntity)} not found");
    }
}
