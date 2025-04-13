using Data.Context;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class ClientRepository(ApplicationDbContext context) : BaseRepository<ClientEntity>(context), IClientRepository 
{
    private readonly ApplicationDbContext _context = context;
}