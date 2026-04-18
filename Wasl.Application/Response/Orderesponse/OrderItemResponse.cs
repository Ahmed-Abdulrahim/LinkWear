namespace Wasl.Application.Response.Orderesponse
{
    public class OrderItemResponse
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string? Details { get; set; }
    }
}
