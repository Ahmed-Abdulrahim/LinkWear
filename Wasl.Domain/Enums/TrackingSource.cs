namespace Wasl.Domain.Enums
{
    public enum TrackingStatus
    {
        OrderPlaced = 1,
        UnderReview = 2,
        AwaitingPayment = 3,
        Paid = 4,
        PickedUp = 5,
        InTransit = 6,
        Delivered = 7,
        FailedDelivery = 8,
        Returned = 9
    }
}
