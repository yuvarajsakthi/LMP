using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(IdDTO userId)
        {
            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId.Id);
            return _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
        }

        public async Task<BoolDTO> DeleteNotificationAsync(IdDTO notificationId, IdDTO userId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId.Id);
            if (notification == null || notification.UserId != userId.Id)
                return new BoolDTO { Value = false };

            await _unitOfWork.Notifications.DeleteAsync(notificationId.Id);
            await _unitOfWork.SaveChangesAsync();
            return new BoolDTO { Value = true };
        }
    }
}
