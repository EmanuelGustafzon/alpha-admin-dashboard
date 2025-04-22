using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public class NotificationService(INotificationRepository notificationRepository, INotificationDissmissRepository notificationDissmissRepository) : INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly INotificationDissmissRepository _notificationDissmissRepository = notificationDissmissRepository;
    public async Task<ServiceResult<Notification>> AddNotficationAsync(NotificationForm form)
    {
        var entity = form.MapTo<NotificationEntity>();
        var result = await _notificationRepository.CreateAsync(entity);
        if (result.Result is null) return ServiceResult<Notification>.Error("Failed to create notification");

        return ServiceResult<Notification>.Created(result.Result.MapTo<Notification>());
    }

    public async Task<ServiceResult<IEnumerable<Notification>>> GetNotificationsAsync(string memberId)
    {
        var dissmissResult = await _notificationDissmissRepository.GetAllAsync(filterBy: x => x.MemberId == memberId);

        if (dissmissResult.Result is null) return ServiceResult<IEnumerable<Notification>>.Error("");

        var list = dissmissResult.Result.Select(x => x.NotificationId).ToList();

        
        var notificationResult = await _notificationRepository.GetAllAsync(filterBy: x => !list.Contains(x.Id));
        if(notificationResult.Result is null) return ServiceResult<IEnumerable<Notification>>.Error("");

        var notifications = notificationResult.Result.Select(x => x.MapTo<Notification>());
        return ServiceResult<IEnumerable<Notification>>.Ok(notifications);
    }

    public async Task<ServiceResult<bool>> DissmissNotification(string memberId, string notificationId)
    {
        var alreadyDissmissedResult = await _notificationDissmissRepository.GetAllAsync(filterBy: x => x.NotificationId == notificationId && x.MemberId == memberId);
        if (alreadyDissmissedResult.Result is null) return ServiceResult<bool>.Error("");

        if (alreadyDissmissedResult.Result.Any()) return ServiceResult<bool>.BadRequest("");

        var dismiss = new NotificationDissmissEntity
            {
                MemberId = memberId,
                NotificationId = notificationId
            };
        var result = await _notificationDissmissRepository.CreateAsync(dismiss);
        if(result.Result is null) return ServiceResult<bool>.Error("");

        return ServiceResult<bool>.Ok(true);
    }
}
