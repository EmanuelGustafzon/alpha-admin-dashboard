using Data.Context;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class NotificationDissmissRepository(ApplicationDbContext context) : BaseRepository<NotificationDissmissEntity>(context), INotificationDissmissRepository
{
    private readonly ApplicationDbContext _context = context;
}
