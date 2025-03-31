using Data.Context;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class MemberRepository(ApplicationDbContext context) : BaseRepository<MemberEntity>(context), IMemberRepository
{
    private readonly ApplicationDbContext _context = context;
}
