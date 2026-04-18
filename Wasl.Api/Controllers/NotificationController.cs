namespace Wasl.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        //Register FCM Token
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterToken(RegisterFcmTokenDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var result = await notificationService.RegisterTokenAsync(userId, dto.Token);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Get Notifications 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var result = await notificationService.GetNotificationsAsync(userId, page, pageSize);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Get Unread Count
        [HttpGet("unread-count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var result = await notificationService.GetUnreadCountAsync(userId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Mark As Read
        [HttpPut("{id}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var result = await notificationService.MarkAsReadAsync(userId, id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Mark All As Read
        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var result = await notificationService.MarkAllAsReadAsync(userId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
