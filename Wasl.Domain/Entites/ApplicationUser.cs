namespace Wasl.Domain.Entites
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? BusinessName { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? City { get; set; }
        public string? BusinessDescription { get; set; }
        public ActivityType? ActivityType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Product> Products { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Order> StoreOrders { get; set; }
        public ICollection<Order> SupplierOrders { get; set; }
        public ICollection<Notifications> Notifications { get; set; }
        public ICollection<FcmToken> FcmToken { get; set; }
    }
}
