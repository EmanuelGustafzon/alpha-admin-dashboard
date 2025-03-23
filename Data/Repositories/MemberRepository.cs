using Data.Context;
using Data.Entities;
using Data.Interfaces;


namespace Data.Repositories;

public class MemberRepository(ApplicationDbContext context) : BaseRepository<MemberEntity>(context), IMemberRepository
{
    private readonly ApplicationDbContext _context = context;
}
