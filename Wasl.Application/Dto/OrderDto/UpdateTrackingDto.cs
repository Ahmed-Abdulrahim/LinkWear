namespace Wasl.Application.Dto.OrderDto
{
    public class UpdateTrackingDto
    {
        public string TrackingNumber { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
    }
}
