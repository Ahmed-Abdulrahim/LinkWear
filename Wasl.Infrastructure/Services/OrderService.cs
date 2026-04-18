namespace Wasl.Infrastructure.Services
{
    public class OrderService(ICurrentUserService currentUser, IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, INotificationService notificationService) : IOrderService
    {

        // StoreOwner creates a new order, selecting a Supplier.
        public async Task<ResultResponse<OrderResponse>> CreateOrderAsync(CreateOrderDto dto)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            if (dto.Items == null || !dto.Items.Any())
                return ResultResponse<OrderResponse>.Failure("Order must contain at least one item.");

            // Validate that the Supplier exists and is approved

            if (currentUser.Role != "StoreOwner")
                return ResultResponse<OrderResponse>.Failure("Canot make Order");

            if (currentUser.IsDeleted)
                return ResultResponse<OrderResponse>.Failure("Supplier account is deactivated.");


            var order = new Order
            {
                StoreOwnerId = Guid.Parse(currentUser.UserId!),
                SupplierId = dto.SupplierId,
                Status = OrderStatus.Pending,
                OrderNumber = GenerateOrderNumber(),
                TrackingNumber = GenerateTrackingNumber(),
                ExpectedDeliveryDate = CalculateExpectedDate(),
                Notes = dto.Notes,
                DeliveryAddress = dto.DeliveryAddress,
                Items = new List<OrderItem>()
            };

            foreach (var item in dto.Items)
            {
                var product = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);

                if (product == null)
                    return ResultResponse<OrderResponse>.Failure($"Product {item.ProductId} not found.");

                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Details = item.Details
                });
            }

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            await unitOfWork.Repository<Order>().AddAsync(order);
            await unitOfWork.CommitAsync();

            // Notify the Supplier about the new order
            await notificationService.SendAndSaveNotificationAsync(
                dto.SupplierId,
                "New Order Received",
                $"You have received a new order {order.OrderNumber}.",
                NotificationType.OrderCreated,
                order.Id);

            var orderResponse = mapper.Map<OrderResponse>(order);
            return ResultResponse<OrderResponse>.Success(orderResponse);
        }


        // StoreOwner cancels a pending order.
        public async Task<ResultResponse<object>> CancelOrderAsync(Guid orderId)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<object>.Failure("Unauthorized");
            var order = await unitOfWork.Repository<Order>().GetByIdTrackedAsync(orderId);

            if (order == null)
                return ResultResponse<object>.Failure("Order not found.");

            if (order.StoreOwnerId != Guid.Parse(currentUser.UserId!))
                return ResultResponse<object>.Failure("Not allowed.");

            if (order.Status != OrderStatus.Pending)
                return ResultResponse<object>.Failure("Cannot cancel after processing has started");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.CommitAsync();

            // Notify the Supplier about the cancellation
            await notificationService.SendAndSaveNotificationAsync(
                order.SupplierId,
                "Order Cancelled",
                $"Order {order.OrderNumber} has been cancelled by the store owner.",
                NotificationType.OrderCancelled,
                order.Id);

            return ResultResponse<object>.Success("Order cancelled successfully.");
        }


        // Get all orders for a specific StoreOwner
        public async Task<ResultResponse<OrderResponse>> GetOrdersAsync()
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            OrderSpecification spec;

            if (currentUser.Role == "StoreOwner")
            {
                spec = new OrderSpecification(p => p.StoreOwnerId == Guid.Parse(currentUser.UserId!));
            }
            else if (currentUser.Role == "Admin")
            {
                spec = new OrderSpecification();
            }
            else
            {
                return ResultResponse<OrderResponse>.Failure("Unauthorized role");
            }
            var orders = await unitOfWork.Repository<Order>().GetAllSpecTrackedAsync(spec);

            if (!orders.Any())
                return ResultResponse<OrderResponse>.Failure("No orders found.");

            var mapped = mapper.Map<List<OrderResponse>>(orders);
            return ResultResponse<OrderResponse>.Success(mapped);
        }


        // Get order details accessible by StoreOwner or Supplier who owns the order.
        public async Task<ResultResponse<OrderResponse>> GetOrderDetails(Guid orderId)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            OrderSpecification spec;

            if (currentUser.Role == "Supplier")
            {
                spec = new OrderSpecification(p => p.Id == orderId && p.SupplierId == Guid.Parse(currentUser.UserId!));
            }
            else if (currentUser.Role == "StoreOwner")
            {
                spec = new OrderSpecification(p => p.Id == orderId && p.StoreOwnerId == Guid.Parse(currentUser.UserId!));
            }
            else if (currentUser.Role == "Admin")
            {
                spec = new OrderSpecification(p => p.Id == orderId);
            }
            else
            {
                return ResultResponse<OrderResponse>.Failure("Unauthorized role");
            }
            var order = await unitOfWork.Repository<Order>().GetByIdSpecAsync(spec);

            if (order is null)
                return ResultResponse<OrderResponse>.Failure("Order not found.");

            var mapped = mapper.Map<OrderResponse>(order);
            mapped.Timeline = BuildTimeline(order.TrackingHistory.ToList());

            return ResultResponse<OrderResponse>.Success(mapped);
        }

        #region Private Methods

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow.Ticks}";
        }

        private string GenerateTrackingNumber()
        {
            return $"TRK-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        private DateTime CalculateExpectedDate()
        {
            return DateTime.UtcNow.AddDays(3);
        }
        private List<TrackingStepDto> BuildTimeline(List<ShipmentTrackingHistory> history)
        {
            var steps = new List<TrackingStatus>
    {
        TrackingStatus.OrderPlaced,
        TrackingStatus.UnderReview,
        TrackingStatus.AwaitingPayment,
        TrackingStatus.Paid,
        TrackingStatus.InTransit,
        TrackingStatus.Delivered
    };

            return steps.Select(step =>
            {
                var h = history.FirstOrDefault(x => x.Status == step);

                return new TrackingStepDto
                {
                    Title = step.ToString(),
                    IsCompleted = h != null,
                    IsCurrent = h?.IsCurrent ?? false,
                    Date = h?.StatusDate
                };
            }).ToList();
        }
        #endregion
    }
}
