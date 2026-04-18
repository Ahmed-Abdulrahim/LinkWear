namespace Wasl.Application.Response.Orderesponse
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Store Owner info
        public string? StoreOwnerId { get; set; }
        public string? StoreOwnerName { get; set; }

        // Supplier info
        public string? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierCity { get; set; }
        public string? SupplierPhone { get; set; }
        public string? SupplierEmail { get; set; }

        // Order details
        public string? Notes { get; set; }
        public string? DeliveryAddress { get; set; }

        // Tracking info
        public string? TrackingNumber { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        public List<OrderItemResponse> Items { get; set; }

        public List<TrackingStepDto> Timeline { get; set; }
    }
}
