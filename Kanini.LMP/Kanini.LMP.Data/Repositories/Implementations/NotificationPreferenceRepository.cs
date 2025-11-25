using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.Data;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class NotificationPreferenceRepository : LMPRepositoy<NotificationPreference, int>, INotificationPreferenceRepository
    {
        public NotificationPreferenceRepository(LmpDbContext context) : base(context)
        {
        }

        public async Task<NotificationPreference?> GetUserPreferenceAsync(int userId, NotificationType type)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type);
        }

        public async Task<IEnumerable<NotificationPreference>> GetUserPreferencesAsync(int userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }
    }
}