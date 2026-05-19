namespace Wasl.Infrastructure.Services
{

    public class SupplierService(ICurrentUserService currentUser, IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, UserManager<ApplicationUser> userManager, ILogger<SupplierService> logger) : ISupplierService
    {
        // Get all orders assigned to this Supplier
        public async Task<ResultResponse<OrderResponse>> GetSupplierOrdersAsync(OrderStatus? status = null)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            OrderSpecification spec;

            if (currentUser.Role == "Supplier")
            {
                var supplierId = Guid.Parse(currentUser.UserId!);
                if (status.HasValue)
                    spec = new OrderSpecification(p => p.SupplierId == supplierId && p.Status == status.Value);
                else
                    spec = new OrderSpecification(p => p.SupplierId == supplierId);
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

        // Get pending orders assigned to this Supplier
        public async Task<ResultResponse<OrderResponse>> GetPendingOrdersAsync()
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            OrderSpecification spec;

            if (currentUser.Role == "Supplier")
            {
                spec = new OrderSpecification(o => o.SupplierId == Guid.Parse(currentUser.UserId!) && o.Status == OrderStatus.Pending);
            }
            else
            {
                return ResultResponse<OrderResponse>.Failure("Unauthorized role");
            }
            var orders = await unitOfWork.Repository<Order>().GetAllSpecTrackedAsync(spec);

            if (!orders.Any())
                return ResultResponse<OrderResponse>.Failure("No pending orders found.");

            var mapped = mapper.Map<List<OrderResponse>>(orders);
            return ResultResponse<OrderResponse>.Success(mapped);
        }

        // Supplier accepts a pending order (adds AwaitingPayment tracking, notifies merchant)
        /* public async Task<ResultResponse<OrderResponse>> AcceptOrderAsync(Guid orderId)
         {
             if (!currentUser.IsAuthenticated)
                 return ResultResponse<OrderResponse>.Failure("Unauthorized");

             if (currentUser.Role != "Supplier")
                 return ResultResponse<OrderResponse>.Failure("Unauthorized role");

             var spec = new OrderSpecification(o => o.Id == orderId && o.SupplierId == Guid.Parse(currentUser.UserId!));
             var order = await unitOfWork.Repository<Order>().GetByIdSpecTrackedAsync(spec);

             if (order is null)
                 return ResultResponse<OrderResponse>.Failure("Order not found or not assigned to you.");

             if (order.Status != OrderStatus.Pending)
                 return ResultResponse<OrderResponse>.Failure("Only pending orders can be accepted.");

             // Add AwaitingPayment tracking entry
             await AddTrackingEntry(order.Id, TrackingStatus.AwaitingPayment, order.Status, order.Status);

             await unitOfWork.CommitAsync();

             logger.LogInformation("Supplier {SupplierId} accepted order {OrderId}",
                 currentUser.UserId, orderId);

             // Notify the StoreOwner
             await notificationService.SendAndSaveNotificationAsync(
                 order.StoreOwnerId,
                 "Order Accepted",
                 $"Your order {order.OrderNumber} has been accepted by the supplier. Please proceed with payment.",
                 NotificationType.OrderStatusUpdated,
                 order.Id);

             var mapped = mapper.Map<OrderResponse>(order);
             return ResultResponse<OrderResponse>.Success(mapped);
         }

         // Supplier rejects a pending order (changes status to Cancelled, notifies merchant)
         public async Task<ResultResponse<OrderResponse>> RejectOrderAsync(Guid orderId)
         {
             if (!currentUser.IsAuthenticated)
                 return ResultResponse<OrderResponse>.Failure("Unauthorized");

             if (currentUser.Role != "Supplier")
                 return ResultResponse<OrderResponse>.Failure("Unauthorized role");

             var spec = new OrderSpecification(o => o.Id == orderId && o.SupplierId == Guid.Parse(currentUser.UserId!));
             var order = await unitOfWork.Repository<Order>().GetByIdSpecTrackedAsync(spec);

             if (order is null)
                 return ResultResponse<OrderResponse>.Failure("Order not found or not assigned to you.");

             if (order.Status != OrderStatus.Pending)
                 return ResultResponse<OrderResponse>.Failure("Only pending orders can be rejected.");

             order.Status = OrderStatus.Cancelled;
             order.UpdatedAt = DateTime.UtcNow;

             await unitOfWork.CommitAsync();

             logger.LogInformation("Supplier {SupplierId} rejected order {OrderId}",
                 currentUser.UserId, orderId);

             // Notify the StoreOwner
             await notificationService.SendAndSaveNotificationAsync(
                 order.StoreOwnerId,
                 "Order Rejected",
                 $"Your order {order.OrderNumber} has been rejected by the supplier.",
                 NotificationType.OrderCancelled,
                 order.Id);

             var mapped = mapper.Map<OrderResponse>(order);
             return ResultResponse<OrderResponse>.Success(mapped);
         }*/

        // Supplier submits a price offer for a pending order (عرض السعر)
        // Sets UnitPrice on all items, saves SupplierNotes, transitions to AwaitingPayment, notifies merchant
        public async Task<ResultResponse<OrderResponse>> SubmitPriceOfferAsync(Guid orderId, SubmitPriceOfferDto dto)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            if (currentUser.Role != "Supplier")
                return ResultResponse<OrderResponse>.Failure("Unauthorized role");

            var spec = new OrderSpecification(o => o.Id == orderId && o.SupplierId == Guid.Parse(currentUser.UserId!));
            var order = await unitOfWork.Repository<Order>().GetByIdSpecTrackedAsync(spec);

            if (order is null)
                return ResultResponse<OrderResponse>.Failure("Order not found or not assigned to you.");

            if (order.Status != OrderStatus.Pending)
                return ResultResponse<OrderResponse>.Failure("Price offer can only be submitted for pending orders.");

            // Update unit price on every item and recalculate total
            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    item.UnitPrice = dto.UnitPrice;
                }
            }
            order.TotalAmount = order.Items?.Sum(i => i.UnitPrice * i.Quantity) ?? 0;

            // Append supplier notes to the existing order notes
            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                    ? $"[Supplier]: {dto.Notes}"
                    : $"{order.Notes}\n[Supplier]: {dto.Notes}";
            }
            order.UpdatedAt = DateTime.UtcNow;

            // Add AwaitingPayment tracking entry
            await AddTrackingEntry(order.Id, TrackingStatus.AwaitingPayment, order.Status, order.Status);

            await unitOfWork.CommitAsync();

            logger.LogInformation("Supplier {SupplierId} submitted price offer for order {OrderId}: UnitPrice={UnitPrice}",
                currentUser.UserId, orderId, dto.UnitPrice);

            // Notify the StoreOwner (merchant) about the price offer
            await notificationService.SendAndSaveNotificationAsync(
                order.StoreOwnerId,
                "Price Offer Received",
                $"The supplier has submitted a price offer for order {order.OrderNumber}. Unit price: {dto.UnitPrice:F2}. Please review and proceed with payment.",
                NotificationType.PriceOfferSubmitted,
                order.Id);

            var mapped = mapper.Map<OrderResponse>(order);
            return ResultResponse<OrderResponse>.Success(mapped);
        }

        // Supplier updates order status. Valid transitions: Paid → Shipped → Delivered.
        public async Task<ResultResponse<OrderResponse>> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto dto)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            OrderSpecification spec;

            if (currentUser.Role == "Supplier")
            {
                spec = new OrderSpecification(o => o.Id == orderId && o.SupplierId == Guid.Parse(currentUser.UserId!));
            }
            else
            {
                return ResultResponse<OrderResponse>.Failure("Unauthorized role");
            }

            var order = await unitOfWork.Repository<Order>().GetByIdSpecTrackedAsync(spec);

            if (order is null)
                return ResultResponse<OrderResponse>.Failure("Order not found or not assigned to you.");

            // Validate status transition
            var validationResult = ValidateStatusTransition(order.Status, dto.NewStatus);
            if (!validationResult.isValid)
                return ResultResponse<OrderResponse>.Failure(validationResult.error);

            var previousStatus = order.Status;
            order.Status = dto.NewStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await AddTrackingEntry(order.Id, MapOrderStatusToTracking(dto.NewStatus), previousStatus, dto.NewStatus);

            // Add tracking history entry for the status change
            var trackingEntry = new ShipmentTrackingHistory
            {
                OrderId = order.Id,
                Status = MapOrderStatusToTracking(dto.NewStatus),
                StatusDescription = $"Order status updated from {previousStatus} to {dto.NewStatus}.",
                StatusDate = DateTime.UtcNow,
                IsCurrent = true
            };

            // Mark previous tracking entries as not current
            if (order.TrackingHistory != null)
            {
                foreach (var history in order.TrackingHistory)
                {
                    history.IsCurrent = false;
                }
            }

            await unitOfWork.CommitAsync();

            logger.LogInformation("Supplier {SupplierId} updated order {OrderId} status to {Status}",
                currentUser.UserId, orderId, dto.NewStatus);

            // Notify the StoreOwner about the status update
            await notificationService.SendAndSaveNotificationAsync(
                order.StoreOwnerId,
                "Order Status Updated",
                $"Your order {order.OrderNumber} status has been updated to {dto.NewStatus}.",
                NotificationType.OrderStatusUpdated,
                order.Id);

            var mapped = mapper.Map<OrderResponse>(order);
            return ResultResponse<OrderResponse>.Success(mapped);
        }

        // Supplier updates tracking number and expected delivery date
        // Triggers notification to the StoreOwner
        public async Task<ResultResponse<OrderResponse>> UpdateTrackingAsync(Guid orderId, UpdateTrackingDto dto)
        {
            if (!currentUser.IsAuthenticated)
                return ResultResponse<OrderResponse>.Failure("Unauthorized");

            OrderSpecification spec;

            if (currentUser.Role == "Supplier")
            {
                spec = new OrderSpecification(o => o.Id == orderId && o.SupplierId == Guid.Parse(currentUser.UserId!));
            }
            else
            {
                return ResultResponse<OrderResponse>.Failure("Unauthorized role");
            }

            var order = await unitOfWork.Repository<Order>().GetByIdSpecTrackedAsync(spec);

            if (order is null)
                return ResultResponse<OrderResponse>.Failure("Order not found or not assigned to you");

            // Only allow tracking updates on orders that are not yet delivered or cancelled
            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                return ResultResponse<OrderResponse>.Failure("Cannot update tracking for delivered or cancelled orders");

            order.TrackingNumber = dto.TrackingNumber;
            order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
            order.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.CommitAsync();

            logger.LogInformation("Supplier {SupplierId} updated tracking for order {OrderId}: {TrackingNumber}",
                currentUser.UserId, orderId, dto.TrackingNumber);

            // Notify StoreOwner about tracking update
            await notificationService.SendAndSaveNotificationAsync(
                order.StoreOwnerId,
                "Tracking Updated",
                $"Tracking info for order {order.OrderNumber} has been updated. Tracking #: {dto.TrackingNumber}.",
                NotificationType.TrackingUpdated,
                order.Id);

            var mapped = mapper.Map<OrderResponse>(order);
            return ResultResponse<OrderResponse>.Success(mapped);
        }

        #region Private Methods

        private static readonly Dictionary<OrderStatus, OrderStatus[]> ValidTransitions = new()
{
    { OrderStatus.Pending, new[] { OrderStatus.Paid } },
    { OrderStatus.Paid, new[] { OrderStatus.Shipped } },
    { OrderStatus.Shipped, new[] { OrderStatus.Delivered } }
};

        private static (bool isValid, string error) ValidateStatusTransition(OrderStatus current, OrderStatus next)
        {
            if (!ValidTransitions.ContainsKey(current))
                return (false, $"Cannot transition from {current}");

            if (!ValidTransitions[current].Contains(next))
                return (false, $"Invalid transition from {current} to {next}");

            return (true, string.Empty);
        }
        private async Task AddTrackingEntry(Guid orderId, TrackingStatus status, OrderStatus prev, OrderStatus current)
        {
            var spec = new ShipmentTrackingHistorySpecification(x => x.OrderId == orderId && x.IsCurrent);
            var repo = unitOfWork.Repository<ShipmentTrackingHistory>();

            var oldEntries = await repo.GetAllSpecTrackedAsync(spec);

            foreach (var item in oldEntries)
                item.IsCurrent = false;

            var newEntry = new ShipmentTrackingHistory
            {
                OrderId = orderId,
                Status = status,
                StatusDate = DateTime.UtcNow,
                IsCurrent = true,
                StatusDescription = $"Order changed from {prev} to {current}"
            };

            await repo.AddAsync(newEntry);
        }
        private TrackingStatus MapOrderStatusToTracking(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => TrackingStatus.OrderPlaced,
                OrderStatus.Paid => TrackingStatus.Paid,
                OrderStatus.Shipped => TrackingStatus.InTransit,
                OrderStatus.Delivered => TrackingStatus.Delivered,
                _ => TrackingStatus.OrderPlaced
            };
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
