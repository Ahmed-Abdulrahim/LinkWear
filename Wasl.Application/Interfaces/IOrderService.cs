namespace Wasl.Application.Interfaces
{
    public interface IOrderService
    {
        // Get all orders for a specific store owner
        Task<ResultResponse<OrderResponse>> GetOrdersAsync();

        // Get order details by StoreOwner or Supplier
        Task<ResultResponse<OrderResponse>> GetOrderDetails(Guid orderId);

        // Store Owner creates a new order selecting a Supplier
        Task<ResultResponse<OrderResponse>> CreateOrderAsync(CreateOrderDto dto);

        // Store Owner cancels a pending order
        Task<ResultResponse<object>> CancelOrderAsync(Guid orderId);
    }
}
