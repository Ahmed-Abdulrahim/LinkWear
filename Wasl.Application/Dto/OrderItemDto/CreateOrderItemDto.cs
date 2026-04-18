namespace Wasl.Application.Dto.OrderItemDto
{
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Details { get; set; }
    }
}
