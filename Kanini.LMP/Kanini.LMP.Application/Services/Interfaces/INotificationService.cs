using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
    }
}
