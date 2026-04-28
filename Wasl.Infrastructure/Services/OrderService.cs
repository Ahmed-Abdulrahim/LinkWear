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

            // Add initial tracking entries (OrderPlaced + UnderReview)
            var trackingRepo = unitOfWork.Repository<ShipmentTrackingHistory>();
            await trackingRepo.AddAsync(new ShipmentTrackingHistory
            {
                OrderId = order.Id,
                Status = TrackingStatus.OrderPlaced,
                StatusDescription = "Order has been placed.",
                StatusDate = DateTime.UtcNow,
                IsCurrent = false
            });
            await trackingRepo.AddAsync(new ShipmentTrackingHistory
            {
                OrderId = order.Id,
                Status = TrackingStatus.UnderReview,
                StatusDescription = "Order is under review by supplier.",
                StatusDate = DateTime.UtcNow,
                IsCurrent = true
            });
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
        public async Task<ResultResponse<OrderResponse>> GetOrdersAsync(OrderStatus? status = null)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            OrderSpecification spec;

            if (currentUser.Role == "StoreOwner")
            {
                var userId = Guid.Parse(currentUser.UserId!);
                if (status.HasValue)
                    spec = new OrderSpecification(p => p.StoreOwnerId == userId && p.Status == status.Value);
                else
                    spec = new OrderSpecification(p => p.StoreOwnerId == userId);
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


        // Merchant dashboard statistics
        public async Task<ResultResponse<MerchantDashboardResponse>> GetMerchantDashboardAsync()
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<MerchantDashboardResponse>.Failure("Unauthorized");

            if (currentUser.Role != "StoreOwner")
                return ResultResponse<MerchantDashboardResponse>.Failure("Unauthorized role");

            var storeOwnerId = Guid.Parse(currentUser.UserId!);

            var orderSpec = new OrderSpecification(o => o.StoreOwnerId == storeOwnerId);
            var orders = await unitOfWork.Repository<Order>().GetAllSpecTrackedAsync(orderSpec);
            var orderList = orders.ToList();

            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var ordersThisMonth = orderList.Count(o => o.CreatedAt >= startOfMonth);
            var underReviewCount = orderList.Count(o => o.Status == OrderStatus.Pending);
            var shippingCount = orderList.Count(o => o.Status == OrderStatus.Shipped);

            var recentOrders = orderList
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber ?? string.Empty,
                    StoreOwnerName = o.Supplier?.BusinessName,
                    ProductName = o.Items?.FirstOrDefault()?.Product?.Name,
                    Quantity = o.Items?.Sum(i => i.Quantity) ?? 0,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                })
                .ToList();

            var dashboard = new MerchantDashboardResponse
            {
                OrdersThisMonth = ordersThisMonth,
                UnderReviewCount = underReviewCount,
                ShippingCount = shippingCount,
                RecentOrders = recentOrders,
            };

            return ResultResponse<MerchantDashboardResponse>.Success(dashboard);
        }


        // Merchant confirms payment (fake — just changes status Pending → Paid)
        public async Task<ResultResponse<OrderResponse>> ConfirmPaymentAsync(Guid orderId)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            if (currentUser.Role != "StoreOwner")
                return ResultResponse<OrderResponse>.Failure("Unauthorized role");

            var storeOwnerId = Guid.Parse(currentUser.UserId!);
            var spec = new OrderSpecification(o => o.Id == orderId && o.StoreOwnerId == storeOwnerId);
            var order = await unitOfWork.Repository<Order>().GetByIdSpecTrackedAsync(spec);

            if (order is null)
                return ResultResponse<OrderResponse>.Failure("Order not found.");

            if (order.Status != OrderStatus.Pending)
                return ResultResponse<OrderResponse>.Failure("Order is not awaiting payment.");

            order.Status = OrderStatus.Paid;
            order.UpdatedAt = DateTime.UtcNow;

            // Add Paid tracking entry
            var trackingSpec = new ShipmentTrackingHistorySpecification(x => x.OrderId == orderId && x.IsCurrent);
            var oldEntries = await unitOfWork.Repository<ShipmentTrackingHistory>().GetAllSpecTrackedAsync(trackingSpec);
            foreach (var item in oldEntries)
                item.IsCurrent = false;

            await unitOfWork.Repository<ShipmentTrackingHistory>().AddAsync(new ShipmentTrackingHistory
            {
                OrderId = order.Id,
                Status = TrackingStatus.Paid,
                StatusDescription = "Payment confirmed.",
                StatusDate = DateTime.UtcNow,
                IsCurrent = true
            });

            await unitOfWork.CommitAsync();

            // Notify the Supplier about the payment
            await notificationService.SendAndSaveNotificationAsync(
                order.SupplierId,
                "Payment Confirmed",
                $"Payment for order {order.OrderNumber} has been confirmed.",
                NotificationType.PaymentSuccess,
                order.Id);

            var mapped = mapper.Map<OrderResponse>(order);
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
