using Data.Context;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class MemberProfileRepository(ApplicationDbContext context) : BaseRepository<MemberProfileEntity>(context), IMemberProfileRepository 
{
    private readonly ApplicationDbContext _context = context;
}
