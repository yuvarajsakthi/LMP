using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface INotificationPreferenceRepository : ILMPRepository<NotificationPreference, int>
    {
        Task<NotificationPreference?> GetUserPreferenceAsync(int userId, NotificationType type);
        Task<IEnumerable<NotificationPreference>> GetUserPreferencesAsync(int userId);
    }
}