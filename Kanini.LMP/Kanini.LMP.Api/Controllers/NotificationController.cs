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
        private readonly IEnhancedNotificationService _enhancedNotificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            IEnhancedNotificationService enhancedNotificationService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _enhancedNotificationService = enhancedNotificationService;
            _logger = logger;
        }

        [HttpGet(ApplicationConstants.Routes.MyNotifications)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetMyNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingNotificationsRetrieval, userId);

                var notifications = await _notificationService.GetUserNotificationsAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.NotificationsRetrievalCompleted, notifications.Count(), userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.NotificationsRetrievalFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.NotificationsRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Unread)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetUnreadNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUnreadNotifications, userId);

                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.UnreadNotificationsCompleted, notifications.Count(), userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UnreadNotificationsFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UnreadNotificationsFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.UnreadCount)]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUnreadCount, userId);

                var count = await _notificationService.GetUnreadCountAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.UnreadCountCompleted, userId, count);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UnreadCountFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UnreadCountFailed });
            }
        }

        [HttpPut(ApplicationConstants.Routes.MarkRead)]
        public async Task<ActionResult<NotificationDTO>> MarkAsRead(int notificationId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingMarkAsRead, notificationId);

                var notification = await _notificationService.MarkAsReadAsync(notificationId);

                _logger.LogInformation(ApplicationConstants.Messages.MarkAsReadCompleted, notificationId);
                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.MarkAsReadFailed, notificationId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.MarkAsReadFailed });
            }
        }

        [HttpPut(ApplicationConstants.Routes.MarkAllRead)]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingMarkAllAsRead, userId);

                var result = await _notificationService.MarkAllAsReadAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.MarkAllAsReadCompleted, userId);
                return Ok(new { success = result, message = ApplicationConstants.ErrorMessages.AllNotificationsMarkedAsRead });
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.MarkAllAsReadFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.MarkAllAsReadFailed });
            }
        }

        // Notification Preferences
        [HttpGet(ApplicationConstants.Routes.Preferences)]
        public async Task<ActionResult> GetNotificationPreferences()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPreferencesRetrieval, userId);

                var preferences = await _enhancedNotificationService.GetUserNotificationPreferencesAsync(userId);

                _logger.LogInformation(ApplicationConstants.Messages.PreferencesRetrievalCompleted, userId);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PreferencesRetrievalFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.PreferencesRetrievalFailed });
            }
        }

        [HttpPut(ApplicationConstants.Routes.PreferencesUpdate)]
        public async Task<ActionResult> UpdateNotificationPreferences(
            NotificationType notificationType,
            [FromBody] NotificationPreferenceUpdateDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPreferencesUpdate, userId, notificationType);

                var result = await _enhancedNotificationService.UpdateNotificationPreferencesAsync(
                    userId, notificationType, request.EmailEnabled, request.SMSEnabled,
                    request.PushEnabled, request.WhatsAppEnabled, request.InAppEnabled);

                if (result)
                {
                    _logger.LogInformation(ApplicationConstants.Messages.PreferencesUpdateCompleted, userId, notificationType);
                    return Ok(new { Message = ApplicationConstants.ErrorMessages.PreferencesUpdatedSuccessfully });
                }

                _logger.LogWarning(ApplicationConstants.ErrorMessages.PreferencesUpdateFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.PreferencesUpdateFailed });
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PreferencesUpdateFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.PreferencesUpdateFailed });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Test)]
        public async Task<ActionResult> SendTestNotification([FromBody] TestNotificationRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingTestNotification, userId);

                await _enhancedNotificationService.SendMultiChannelNotificationAsync(
                    userId, request.Title ?? ApplicationConstants.Messages.Unknown,
                    request.Message ?? ApplicationConstants.Messages.Unknown,
                    NotificationType.General, NotificationPriority.Low);

                _logger.LogInformation(ApplicationConstants.Messages.TestNotificationCompleted, userId);
                return Ok(new { Message = ApplicationConstants.ErrorMessages.TestNotificationSent });
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.TestNotificationFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.TestNotificationFailed });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Bulk)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult> SendBulkNotification([FromBody] BulkNotificationRequest request)
        {
            try
            {
                if (!request.UserIds.Any())
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.UserIdsRequired);
                    return BadRequest(new { message = ApplicationConstants.ErrorMessages.UserIdsRequired });
                }

                _logger.LogInformation(ApplicationConstants.Messages.ProcessingBulkNotification, request.UserIds.Count);

                await _enhancedNotificationService.SendBulkNotificationAsync(
                    request.UserIds, request.Title, request.Message, request.Type, request.Priority);

                _logger.LogInformation(ApplicationConstants.Messages.BulkNotificationCompleted, request.UserIds.Count);
                return Ok(new { Message = string.Format(ApplicationConstants.ErrorMessages.BulkNotificationSent, request.UserIds.Count) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.BulkNotificationFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.BulkNotificationFailed });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }

    public class NotificationPreferenceUpdateDto
    {
        public bool EmailEnabled { get; set; } = true;
        public bool SMSEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = false;
        public bool WhatsAppEnabled { get; set; } = false;
        public bool InAppEnabled { get; set; } = true;
    }

    public class TestNotificationRequest
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
    }

    public class BulkNotificationRequest
    {
        public List<int> UserIds { get; set; } = new();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.General;
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
    }
}
