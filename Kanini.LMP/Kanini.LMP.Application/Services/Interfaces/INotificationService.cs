using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(IdDTO userId);
        Task<NotificationDTO> CreateNotificationAsync(NotificationDTO notificationDto);
        Task<BoolDTO> DeleteNotificationAsync(IdDTO notificationId, IdDTO userId);
    }
}
