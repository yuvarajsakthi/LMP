using Kanini.LMP.Database.Entities;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface INotificationRepository : ILMPRepository<Notification, int>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
    }
}