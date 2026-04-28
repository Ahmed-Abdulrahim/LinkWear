namespace Wasl.Domain.Entites
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; }
        public Guid StoreOwnerId { get; set; }
        public Guid SupplierId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        //Navigation
        public ApplicationUser StoreOwner { get; set; }
        public ApplicationUser Supplier { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public ICollection<ShipmentTrackingHistory> TrackingHistory { get; set; }

    }
}
