using Business.Models;
using Data.Entities;
using Domain.Models;

namespace Business.Interfaces;

public interface INotificationService
{
    public Task<ServiceResult<Notification>> AddNotficationAsync(NotificationForm form);

    public Task<ServiceResult<IEnumerable<Notification>>> GetNotificationsAsync(string memberId, string target = "All");

    public Task<ServiceResult<bool>> DissmissNotification(string memberId, string notificationId);
}
