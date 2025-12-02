using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.NotificationController)]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet(ApplicationConstants.Routes.MyNotifications)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetMyNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingNotificationsRetrieval);

                var notifications = await _notificationService.GetUserNotificationsAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.NotificationsRetrievalCompleted);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.NotificationsRetrievalFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.NotificationsRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Unread)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetUnreadNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUnreadNotifications);

                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.UnreadNotificationsCompleted);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UnreadNotificationsFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UnreadNotificationsFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.UnreadCount)]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUnreadCount);

                var count = await _notificationService.GetUnreadCountAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.UnreadCountCompleted);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UnreadCountFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UnreadCountFailed });
            }
        }

        [HttpPut(ApplicationConstants.Routes.MarkRead)]
        public async Task<ActionResult<NotificationDTO>> MarkAsRead(int notificationId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingMarkAsRead);

                var notification = await _notificationService.MarkAsReadAsync(notificationId);

                _logger.LogInformation(ApplicationConstants.Messages.MarkAsReadCompleted);
                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.MarkAsReadFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.MarkAsReadFailed });
            }
        }

        [HttpPut(ApplicationConstants.Routes.MarkAllRead)]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingMarkAllAsRead);

                var result = await _notificationService.MarkAllAsReadAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.MarkAllAsReadCompleted);
                return Ok(new { success = result, message = ApplicationConstants.ErrorMessages.AllNotificationsMarkedAsRead });
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.MarkAllAsReadFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.MarkAllAsReadFailed });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}