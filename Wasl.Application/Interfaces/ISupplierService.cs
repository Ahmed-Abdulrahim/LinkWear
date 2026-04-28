namespace Wasl.Application.Interfaces
{
    public interface ISupplierService
    {
        // Get all orders assigned to Supplier.
        Task<ResultResponse<OrderResponse>> GetSupplierOrdersAsync(OrderStatus? status = null);

        /// Get pending orders assigned to Supplier
        Task<ResultResponse<OrderResponse>> GetPendingOrdersAsync();

        // Supplier accepts a pending order (adds AwaitingPayment tracking)
        Task<ResultResponse<OrderResponse>> AcceptOrderAsync(Guid orderId);

        // Supplier rejects a pending order (changes status to Cancelled)
        Task<ResultResponse<OrderResponse>> RejectOrderAsync(Guid orderId);

        // Supplier submits a price offer (unit price + notes) for a pending order
        // Triggers a notification to the StoreOwner
        Task<ResultResponse<OrderResponse>> SubmitPriceOfferAsync(Guid orderId, SubmitPriceOfferDto dto);

        // Supplier updates order status (Paid → Shipped → Delivered)
        // Triggers a notification to the StoreOwner
        Task<ResultResponse<OrderResponse>> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto dto);

        // Supplier updates tracking number and expected delivery date
        // Triggers a notification to the StoreOwner
        Task<ResultResponse<OrderResponse>> UpdateTrackingAsync(Guid orderId, UpdateTrackingDto dto);
    }
}
