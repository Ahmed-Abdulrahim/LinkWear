namespace Wasl.Application.Response.NotificationResponse
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public Guid? OrderId { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
