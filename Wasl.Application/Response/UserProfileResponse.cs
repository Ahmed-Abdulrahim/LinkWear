namespace Wasl.Application.Response
{
    public class UserProfileResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsDeleted { get; set; }
        public string Status { get; set; }
        public string? BusinessName { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }

        // ✅ إضافات
        public ActivityType? ActivityType { get; set; }
        public string? City { get; set; }
        public string? BusinessDescription { get; set; }
    }
}