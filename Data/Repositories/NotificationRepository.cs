using Data.Context;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class NotificationRepository(ApplicationDbContext context) : BaseRepository<NotificationEntity>(context), INotificationRepository
{
    private readonly ApplicationDbContext _context = context;
}
