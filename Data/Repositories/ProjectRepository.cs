using Data.Context;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class ProjectRepository(ApplicationDbContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{
    private readonly ApplicationDbContext _context = context;
}
