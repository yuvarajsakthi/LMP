using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet(ApiConstants.Routes.NotificationController.GetAll)]
        public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDTO>>>> GetAllNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetUserNotificationsAsync(new IdDTO { Id = userId });
                return Ok(ApiResponse<IEnumerable<NotificationDTO>>.SuccessResponse(notifications));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<IEnumerable<NotificationDTO>>.ErrorResponse("Failed to retrieve notifications"));
            }
        }

        [HttpDelete(ApiConstants.Routes.NotificationController.Delete)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteNotification(int notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _notificationService.DeleteNotificationAsync(new IdDTO { Id = notificationId }, new IdDTO { Id = userId });
                
                if (!result.Value)
                    return NotFound(ApiResponse<object>.ErrorResponse("Notification not found"));
                
                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Notification deleted successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to delete notification"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}