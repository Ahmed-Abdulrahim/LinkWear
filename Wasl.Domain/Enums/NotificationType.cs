namespace Wasl.Domain.Enums
{

    public enum NotificationType
    {
        OrderCreated = 1,
        OrderStatusUpdated = 2,
        TrackingUpdated = 3,
        SupplierApproved = 4,
        SupplierRejected = 5,
        OrderCancelled = 6,
        PaymentSuccess = 7,
        PaymentFailed = 8,
        PriceOfferSubmitted = 9
    }
}
