namespace Wasl.Domain.Entites
{
    public class Notifications : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; } = false;
        public Guid? OrderId { get; set; }
        public NotificationType Type { get; set; }

        //Navigation Prop
        public ApplicationUser User { get; set; }
        public Order Order { get; set; }


    }
}
