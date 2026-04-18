namespace Wasl.Application.Interfaces
{
    public interface ISupplierService
    {
        // Get all orders assigned to Supplier.
        Task<ResultResponse<OrderResponse>> GetSupplierOrdersAsync();

        /// Get pending orders assigned to Supplier
        Task<ResultResponse<OrderResponse>> GetPendingOrdersAsync();

        // Supplier updates order status (Pending → Preparing → Shipped → Delivered)
        // Triggers a notification to the StoreOwner
        Task<ResultResponse<OrderResponse>> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto dto);

        // Supplier updates tracking number and expected delivery date
        // Triggers a notification to the StoreOwner
        Task<ResultResponse<OrderResponse>> UpdateTrackingAsync(Guid orderId, UpdateTrackingDto dto);
    }
}
