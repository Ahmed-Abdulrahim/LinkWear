namespace Wasl.Application.Dto.OrderDto
{
    public class CreateOrderDto
    {
        public Guid SupplierId { get; set; }

        // Additional order details
        public string? Notes { get; set; }

        // Delivery address for the store
        public string? DeliveryAddress { get; set; }

        public List<CreateOrderItemDto> Items { get; set; }
    }
}
