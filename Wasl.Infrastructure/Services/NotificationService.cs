namespace Wasl.Infrastructure.Services
{
    public class NotificationService(IUnitOfWork unitOfWork, IFcmService fcmService, IMapper mapper) : INotificationService
    {
        //Register FCM Token
        public async Task<ResultResponse<object>> RegisterTokenAsync(string userId, string token)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return ResultResponse<object>.Failure("Invalid user ID format.");

            // Check for duplicate token
            var duplicateSpec = new FcmTokenSpecification(parsedUserId, token);
            var existingToken = await unitOfWork.Repository<FcmToken>().GetByIdSpecAsync(duplicateSpec);

            if (existingToken is not null)
                return ResultResponse<object>.Success("Token already registered.");

            var fcmToken = new FcmToken
            {
                UserId = parsedUserId,
                Token = token
            };

            await unitOfWork.Repository<FcmToken>().AddAsync(fcmToken);
            await unitOfWork.CommitAsync();
            return ResultResponse<object>.Success("Token registered successfully.");
        }

        //Get Notifications With Pagination
        public async Task<ResultResponse<NotificationResponse>> GetNotificationsAsync(string userId, int page, int pageSize)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return ResultResponse<NotificationResponse>.Failure("Invalid user ID format.");

            var spec = new NotificationSpecification(parsedUserId, page, pageSize);
            var notifications = await unitOfWork.Repository<Notifications>().GetAllSpecTrackedAsync(spec);

            var mapped = mapper.Map<List<NotificationResponse>>(notifications);
            return ResultResponse<NotificationResponse>.Success(mapped);
        }

        //Get Unread Count
        public async Task<ResultResponse<int>> GetUnreadCountAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return ResultResponse<int>.Failure("Invalid user ID format.");

            var spec = new NotificationSpecification(n => n.UserId == parsedUserId && !n.IsRead && !n.IsDeleted);
            var unreadNotifications = await unitOfWork.Repository<Notifications>().GetAllSpecTrackedAsync(spec);
            var count = unreadNotifications.Count();

            return ResultResponse<int>.Success(count);
        }

        //Mark As Read
        public async Task<ResultResponse<object>> MarkAsReadAsync(string userId, Guid notificationId)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return ResultResponse<object>.Failure("Invalid user ID format.");

            var notification = await unitOfWork.Repository<Notifications>().GetByIdTrackedAsync(notificationId);

            if (notification is null)
                return ResultResponse<object>.Failure("Notification not found.");

            if (notification.UserId != parsedUserId)
                return ResultResponse<object>.Failure("Not allowed.");

            if (notification.IsRead)
                return ResultResponse<object>.Success("Notification already marked as read.");

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.CommitAsync();

            return ResultResponse<object>.Success("Notification marked as read.");
        }

        //Mark All As Read
        public async Task<ResultResponse<object>> MarkAllAsReadAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return ResultResponse<object>.Failure("Invalid user ID format.");

            var spec = new NotificationSpecification(n => n.UserId == parsedUserId && !n.IsRead && !n.IsDeleted);
            var unreadNotifications = await unitOfWork.Repository<Notifications>().GetAllSpecTrackedAsync(spec);

            if (!unreadNotifications.Any())
                return ResultResponse<object>.Success("No unread notifications.");

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await unitOfWork.CommitAsync();
            return ResultResponse<object>.Success("All notifications marked as read.");
        }

        public async Task SendAndSaveNotificationAsync(Guid userId, string title, string body, NotificationType type, Guid? orderId = null)
        {
            var notification = new Notifications
            {
                UserId = userId,
                Title = title,
                Body = body,
                IsRead = false,
                OrderId = orderId,
                Type = type
            };

            await unitOfWork.Repository<Notifications>().AddAsync(notification);
            await unitOfWork.CommitAsync();

            var tokenSpec = new FcmTokenSpecification(userId);
            var fcmTokens = await unitOfWork.Repository<FcmToken>().GetAllSpecTrackedAsync(tokenSpec);
            var tokenList = fcmTokens.Select(t => t.Token).ToList();

            if (tokenList.Any())
            {
                await fcmService.SendAsync(tokenList, title, body);
            }
        }
    }
}
