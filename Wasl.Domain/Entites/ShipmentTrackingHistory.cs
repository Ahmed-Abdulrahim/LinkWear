namespace Wasl.Domain.Entites
{
    public class ShipmentTrackingHistory : BaseEntity
    {
        public Guid OrderId { get; set; }
        public TrackingStatus Status { get; set; }
        public string? StatusDescription { get; set; }
        public DateTime StatusDate { get; set; }
        public bool IsCurrent { get; set; }

        //Navigation
        public Order Order { get; set; }

    }
}
