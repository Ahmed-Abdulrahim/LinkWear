namespace Wasl.Application.Response
{
    public class SupplierResponse
    {
        public string SupplierId { get; set; } = string.Empty;
        public string? BusinessName { get; set; }
        public string? City { get; set; }
        public ActivityType? ActivityType { get; set; }
        public string? BusinessDescription { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
