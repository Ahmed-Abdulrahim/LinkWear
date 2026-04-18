namespace Wasl.Infrastructure.Persistence.DbContext
{
    public class WaslDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public WaslDbContext(DbContextOptions<WaslDbContext> options) : base(options) { }

        // DbSets
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShipmentTrackingHistory> ShipmentTrackingHistory { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<FcmToken> FcmTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(WaslDbContext).Assembly);
        }
    }
}
