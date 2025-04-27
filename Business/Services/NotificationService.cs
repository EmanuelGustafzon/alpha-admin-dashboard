using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using System.Diagnostics;

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

    public async Task<ServiceResult<IEnumerable<Notification>>> GetNotificationsAsync(string memberId, string target = "All")
    {
        var dissmissResult = await _notificationDissmissRepository.GetAllAsync(filterBy: x => x.MemberId == memberId);

        if (dissmissResult.Result is null) return ServiceResult<IEnumerable<Notification>>.Error("Failed to fetch notifications");

        var list = dissmissResult.Result.Select(x => x.NotificationId).ToList();

        
        var notificationResult = await _notificationRepository.GetAllAsync(filterBy: x => !list.Contains(x.Id) && (x.Target == target || x.Target == "All"), orderByDecending: true, sortBy: x => x.Created);
        if(notificationResult.Result is null) return ServiceResult<IEnumerable<Notification>>.Error("Failed to fecth notifications");

        var notifications = notificationResult.Result.Select(x => x.MapTo<Notification>());
        return ServiceResult<IEnumerable<Notification>>.Ok(notifications);
    }

    public async Task<ServiceResult<bool>> DissmissNotification(string memberId, string notificationId)
    {
        var alreadyDissmissedResult = await _notificationDissmissRepository.GetAllAsync(filterBy: x => x.NotificationId == notificationId && x.MemberId == memberId);
        if (alreadyDissmissedResult.Result is null) return ServiceResult<bool>.Error("Failed to fetch dissmissed notifications");

        if (alreadyDissmissedResult.Result.Any()) return ServiceResult<bool>.BadRequest("You have already dissmissed this notification");

        var dismiss = new NotificationDissmissEntity
            {
                MemberId = memberId,
                NotificationId = notificationId
            };
        var result = await _notificationDissmissRepository.CreateAsync(dismiss);
        if(result.Result is null) return ServiceResult<bool>.Error("Failed to created notification dissmissal");

        await DeleteIfAllDissmissed(notificationId);

        return ServiceResult<bool>.Ok(true);
    }

    private async Task DeleteIfAllDissmissed(string notificationId)
    {
        try
        {
            var dissmissedEntitiesResult = await _notificationDissmissRepository.GetAllAsync(filterBy: x => x.NotificationId == notificationId);
            if (!dissmissedEntitiesResult.Succeeded) return;

            if(dissmissedEntitiesResult.Result is IEnumerable<NotificationDissmissEntity> && dissmissedEntitiesResult.Result.Count() == 0)
            {
                await _notificationRepository.DeleteAsync(x => x.Id  == notificationId);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
