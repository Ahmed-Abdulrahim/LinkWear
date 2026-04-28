namespace Wasl.Application.Response
{
    public class MerchantDashboardResponse
    {
        public int OrdersThisMonth { get; set; }
        public int UnderReviewCount { get; set; }
        public int ShippingCount { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }
}
