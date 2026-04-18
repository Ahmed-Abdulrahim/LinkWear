namespace Wasl.Application.Interfaces
{
    public interface INotificationService
    {
        Task<ResultResponse<object>> RegisterTokenAsync(string userId, string token);
        Task<ResultResponse<NotificationResponse>> GetNotificationsAsync(string userId, int page, int pageSize);
        Task<ResultResponse<int>> GetUnreadCountAsync(string userId);
        Task<ResultResponse<object>> MarkAsReadAsync(string userId, Guid notificationId);
        Task<ResultResponse<object>> MarkAllAsReadAsync(string userId);

        // Send and save a notification
        Task SendAndSaveNotificationAsync(Guid userId, string title, string body, NotificationType type, Guid? orderId = null);
    }
}
