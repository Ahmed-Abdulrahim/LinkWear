namespace Wasl.Application.Response
{
    public class DashboardResponse
    {
        public int ProductCount { get; set; }
        public int CurrentOrderCount { get; set; }
        public int NewOrderCount { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    public class RecentOrderDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string? StoreOwnerName { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
